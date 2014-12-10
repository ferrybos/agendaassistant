using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vluchtprikker.Entities;
using DB = Vluchtprikker.DB;
using Vluchtprikker.Repositories;
using Vluchtprikker.Shared;

namespace Vluchtprikker.Services
{
    public interface IEventService
    {
        Event Get(string id);
        Event Create(string title, string description, string name, string email, bool addOrganizerAsParticipant);

        void Complete(Event evn);
        void Confirm(string id);

        void SelectFlight(string eventId, long flightSearchId, long flightId);
        Event RefreshFlights(string eventId);
        void SendReminder(string eventId);
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
        private readonly IFlightRepository _flightRepository;

        public EventService(IDbContext dbContext, IMailService mailService, IFlightRepository flightRepository)
        {
            _repository = new EventRepository(dbContext);

            _dbContext = dbContext;
            _mailService = mailService;
            _flightRepository = flightRepository;
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
                {
                    if (dbParticipant.Email.Equals(dbEvent.OrganizerEmail)) continue;
                    _mailService.SendInvitation(dbEvent, dbParticipant);
                }

                //_mailService.SendInvitationConfirmation(dbEvent);

                dbEvent.StatusID = EventStatusEnum.InvitationsSent;
                _dbContext.Current.SaveChanges();
            }
        }

        public void Complete(Event evn)
        {
            var dbEvent = _repository.Single(GuidUtil.ToGuid(evn.Id));

            // update event details
            dbEvent.Title = evn.Title;
            dbEvent.Description = evn.Description;
            dbEvent.OrganizerName = evn.OrganizerName;
            dbEvent.OrganizerEmail = evn.OrganizerEmail;
            
            // add selected flights
            dbEvent.OutboundFlightSearch = AddFlightSearch(evn.OutboundFlights, evn.Origin, evn.Destination, evn.BeginDate, evn.EndDate, evn.DaysOfWeek, evn.MaxPrice);
            dbEvent.InboundFlightSearch = AddFlightSearch(evn.InboundFlights, evn.Destination, evn.Origin, evn.BeginDate, evn.EndDate, evn.DaysOfWeek, evn.MaxPrice);
            
            // create avaiablities for all participants
            var availabilityRepository = new AvailabilityRepository(_dbContext);
            foreach (var dbParticipant in dbEvent.Participants)
            {
                bool isOrganizer = dbParticipant.Email.Equals(dbEvent.OrganizerEmail);

                short? value = isOrganizer ? (short?)100 : null;

                foreach (var dbFlight in dbEvent.OutboundFlightSearch.Flights)
                    availabilityRepository.Create(dbParticipant, dbFlight, value);
                foreach (var dbFlight in dbEvent.InboundFlightSearch.Flights)
                    availabilityRepository.Create(dbParticipant, dbFlight, value);

                if (isOrganizer)
                {
                    dbParticipant.AvailabilityConfirmed = true;
                }
            }

            dbEvent.StatusID = EventStatusEnum.NewCompleted;
            _dbContext.Current.SaveChanges();

            // send confirmation email
            _mailService.SendEventConfirmation(dbEvent);
        }

        private DB.FlightSearch AddFlightSearch(IEnumerable<Flight> flights, string origin, string destination, DateTime beginDate, DateTime endDate, short daysOfWeek, short? maxPrice)
        {
            var flightSearchRepository = new FlightSearchRepository(_dbContext);

            var dbFlightSearch = flightSearchRepository.Add(
                origin,
                destination,
                beginDate,
                endDate,
                daysOfWeek,
                maxPrice);

            foreach (var flight in flights)
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
                var bitArray = new BitArray(BitConverter.GetBytes(dbEvent.OutboundFlightSearch.DaysOfWeek));
                var response = _flightRepository.Search(dbEvent.OutboundFlightSearch.DepartureStation, dbEvent.OutboundFlightSearch.ArrivalStation, dbEvent.OutboundFlightSearch.StartDate, dbEvent.OutboundFlightSearch.EndDate, (short)dbEvent.Participants.Count, bitArray, (short?)dbEvent.OutboundFlightSearch.MaxPrice);

                UpdateFlights(dbEvent.OutboundFlightSearch, response.OutboundFlights);
                UpdateFlights(dbEvent.InboundFlightSearch, response.InboundFlights);

                _dbContext.Current.SaveChanges();
            }

            return EntityMapper.Map(dbEvent, includeFlights: true, includeParticipants:false, includeAvailability: false);
        }

        public void SendReminder(string eventId)
        {
            var dbEvent = _repository.Single(GuidUtil.ToGuid(eventId));

            foreach (var dbParticipant in dbEvent.Participants)
            {
                _mailService.SendReminder(dbEvent, dbParticipant);
            }

            //dbEvent.StatusID = EventStatusEnum.PushpinCompleted;
            //_dbContext.Current.SaveChanges();
        }

        public void SetPnr(string id, string pnr)
        {
            var dbEvent = _repository.Single(GuidUtil.ToGuid(id));

            if (!string.IsNullOrWhiteSpace(dbEvent.PNR))
                throw new FormattedException("Event already has a PNR");

            dbEvent.PNR = pnr.ToUpper();
            dbEvent.StatusID = EventStatusEnum.BookingCompleted;
            _dbContext.Current.SaveChanges();

            foreach (var dbParticipant in dbEvent.Participants)
            {
                _mailService.SendBookingCreatedEmail(dbEvent, dbParticipant);
            }
        }

        private void UpdateFlights(DB.FlightSearch flightSearch, List<Flight> flights)
        {
            foreach (var dbFlight in flightSearch.Flights)
            {
                Flight updatedFlight = null;

                if (dbFlight.DepartureDate >= DateTime.Today)
                {
                    updatedFlight = flights.SingleOrDefault(
                        f =>
                        f.DepartureDate.Equals(dbFlight.DepartureDate) && f.CarrierCode.Equals(dbFlight.CarrierCode) &&
                        f.FlightNumber.Equals((short) dbFlight.FlightNumber));
                }

                if (updatedFlight != null)
                {
                    dbFlight.STD = updatedFlight.STD;
                    dbFlight.STA = updatedFlight.STA;
                    dbFlight.Price = (int)(updatedFlight.Price * 100);
                    dbFlight.Enabled = true;
                }
                else
                {
                    dbFlight.Enabled = false;
                }
            }
        }
    }
}
