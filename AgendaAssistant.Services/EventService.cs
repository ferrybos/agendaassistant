using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.DB;
using AgendaAssistant.Repositories;
using AgendaAssistant.Shared;
using Event = AgendaAssistant.Entities.Event;

namespace AgendaAssistant.Services
{
    public interface IEventService
    {
        Event Get(string id);
        Event Create(string title, string description, string name, string email, bool addOrganizerAsParticipant);

        void Complete(Event evn);
        void Confirm(string id);

        void SelectFlight(string eventId, long flightSearchId, long flightId);
        Event RefreshFlights(string eventId);
        void ConfirmFlightsToParticipants(string id);
        void SetPnr(string id, string pnr);
    }

    /// <summary>
    /// Contains all business logic regarding events
    /// </summary>
    public class EventService : IEventService
    {
        private readonly EventRepository _repository;
        private readonly IDbContext _dbContext;
        private readonly IMailService _mailService;
        private readonly IFlightService _flightService;

        public EventService(IDbContext dbContext, IMailService mailService, IFlightService flightService)
        {
            _repository = new EventRepository(dbContext);

            _dbContext = dbContext;
            _mailService = mailService;
            _flightService = flightService;
        }

        public Event Get(string id)
        {
            // fetch event (including complete tree)
            var dbEvent = _repository.Single(GuidUtil.ToGuid(id));
            var eventIsActive = dbEvent.PNR == null;

            var evn = EntityMapper.Map(dbEvent, includeAvailability: eventIsActive);

            return evn;
        }

        public Event Create(string title, string description, string name, string email, bool addOrganizerAsParticipant)
        {
            var dbEvent = _repository.Create(title, description, name, email);

            // by default, add organizer as participant
            if (addOrganizerAsParticipant)
            {
                var dbParticipant = new ParticipantRepository(_dbContext).Add(dbEvent.ID, name, email);
                dbEvent.Participants.Add(dbParticipant);
            }

            return EntityMapper.Map(dbEvent);
        }

        public void Confirm(string id)
        {
            var dbEvent = _repository.Single(GuidUtil.ToGuid(id));

            if (_repository.Confirm(GuidUtil.ToGuid(id)))
            {
                foreach (var dbParticipant in dbEvent.Participants)
                    _mailService.SendInvitation(dbEvent, dbParticipant);

                _mailService.SendInvitationConfirmation(dbEvent);

                dbEvent.StatusID = EventStatusEnum.InvitationsSent;
                _dbContext.Current.SaveChanges();
            }
        }

        public void Complete(Event evn)
        {
            var dbEvent = _repository.Single(GuidUtil.ToGuid(evn.Id));

            dbEvent.Title = evn.Title;
            dbEvent.Description = evn.Description;
            dbEvent.OrganizerName = evn.OrganizerName;
            dbEvent.OrganizerEmail = evn.OrganizerEmail;

            dbEvent.OutboundFlightSearch = AddFlightSearch(evn.OutboundFlightSearch);
            dbEvent.InboundFlightSearch = AddFlightSearch(evn.InboundFlightSearch);
            dbEvent.StatusID = EventStatusEnum.NewCompleted;
            _dbContext.Current.SaveChanges();

            // send confirmation email
            _mailService.SendEventConfirmation(dbEvent);
        }

        private FlightSearch AddFlightSearch(Entities.FlightSearch flightSearch)
        {
            var flightSearchRepository = new FlightSearchRepository(_dbContext);

            var dbFlightSearch = flightSearchRepository.Add(
                flightSearch.DepartureStation,
                flightSearch.ArrivalStation,
                flightSearch.BeginDate,
                flightSearch.EndDate,
                flightSearch.DaysOfWeek,
                flightSearch.MaxPrice);

            foreach (var flight in flightSearch.Flights)
            {
                flightSearchRepository.AddFlight(dbFlightSearch, flight.CarrierCode, flight.FlightNumber,
                                                 flight.DepartureDate, flight.STA, flight.STD, (int)(flight.Price*100));
            }

            return dbFlightSearch;
        }

        public void SelectFlight(string eventId, long flightSearchId, long flightId)
        {
            _repository.Single(GuidUtil.ToGuid(eventId));

            var dbFlightySearch = new FlightSearchRepository(_dbContext).Single(flightSearchId);

            dbFlightySearch.SelectedFlightID = flightId == 0 ? (long?)null : flightId;
            _dbContext.Current.SaveChanges();
        }

        public Event RefreshFlights(string eventId)
        {
            var dbEvent = _repository.Single(GuidUtil.ToGuid(eventId));

            bool isRefreshAllowed = string.IsNullOrEmpty(dbEvent.PNR) &&
                                    (!dbEvent.LastModifiedPricesUtc.HasValue ||
                                     dbEvent.LastModifiedPricesUtc.Value.AddMinutes(30) < DateTime.UtcNow);

            if (isRefreshAllowed)
            {
                dbEvent.LastModifiedPricesUtc = DateTime.UtcNow;

                // todo: async
                UpdateFlights(dbEvent.OutboundFlightSearch, (short) dbEvent.Participants.Count);
                UpdateFlights(dbEvent.InboundFlightSearch, (short) dbEvent.Participants.Count);

                _dbContext.Current.SaveChanges();
            }

            return EntityMapper.Map(dbEvent);
        }

        public void ConfirmFlightsToParticipants(string id)
        {
            var dbEvent = _repository.Single(GuidUtil.ToGuid(id));

            foreach (var dbParticipant in dbEvent.Participants)
            {
                _mailService.SendFlightsConfirmation(dbEvent, dbParticipant);
            }

            dbEvent.StatusID = EventStatusEnum.PushpinCompleted;
            _dbContext.Current.SaveChanges();
        }

        public void SetPnr(string id, string pnr)
        {
            var dbEvent = _repository.Single(GuidUtil.ToGuid(id));

            if (!string.IsNullOrWhiteSpace(dbEvent.PNR))
                throw new FormattedException("Event already has a PNR");

            dbEvent.PNR = pnr;
            dbEvent.StatusID = EventStatusEnum.BookingCompleted;
            _dbContext.Current.SaveChanges();

            foreach (var dbParticipant in dbEvent.Participants)
            {
                _mailService.SendBookingCreatedEmail(dbEvent, dbParticipant);
            }
        }

        private void UpdateFlights(FlightSearch flightSearch, short paxCount)
        {
            var bitArray = new BitArray(BitConverter.GetBytes(flightSearch.DaysOfWeek));
            var updatedFlights = _flightService.Search(flightSearch.DepartureStation, flightSearch.ArrivalStation, flightSearch.StartDate, flightSearch.EndDate, paxCount, bitArray, (short?)flightSearch.MaxPrice);

            foreach (var dbFlight in flightSearch.Flights)
            {
                Entities.Flight updatedFlight = null;

                if (dbFlight.DepartureDate >= DateTime.Today)
                {
                    updatedFlight = updatedFlights.SingleOrDefault(
                        f =>
                        f.DepartureDate.Equals(dbFlight.DepartureDate) && f.CarrierCode.Equals(dbFlight.CarrierCode) &&
                        f.FlightNumber.Equals((short) dbFlight.FlightNumber));
                }

                if (updatedFlight != null)
                {
                    dbFlight.STD = updatedFlight.STD;
                    dbFlight.STA = updatedFlight.STA;
                    dbFlight.Price = (int)(updatedFlight.Price * 100);
                    dbFlight.Status = "";
                }
                else
                {
                    dbFlight.Status = "Niet meer beschikbaar";
                }
            }

            _dbContext.Current.SaveChanges();
        }
    }
}
