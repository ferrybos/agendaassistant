using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        Event Create(string title, string description, string name, string email);

        void Complete(Event evn);
        void Confirm(string id);

        void SelectFlight(string eventId, long flightSearchId, long flightId);
        Event RefreshFlights(string eventId);
        void ConfirmFlightsToParticipants(string id);
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

            return EntityMapper.Map(dbEvent);
        }

        public Event Create(string title, string description, string name, string email)
        {
            var dbOrganizerPerson = new PersonRepository(_dbContext).AddOrGetExisting(name, email);
            var dbEvent = _repository.Create(title, description, dbOrganizerPerson);

            // by default, add organizer as participant
            new ParticipantRepository(_dbContext).Add(dbEvent.ID, dbOrganizerPerson.ID);

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
            
            // todo: async
            UpdateFlights(dbEvent.OutboundFlightSearch, (short)dbEvent.Participants.Count);
            UpdateFlights(dbEvent.InboundFlightSearch, (short)dbEvent.Participants.Count);

            _dbContext.Current.SaveChanges();

            return EntityMapper.Map(dbEvent);
        }

        public void ConfirmFlightsToParticipants(string id)
        {
            var dbEvent = _repository.Single(GuidUtil.ToGuid(id));

            foreach (var dbParticipant in dbEvent.Participants)
            {
                _mailService.SendFlightsConfirmation(dbEvent, dbParticipant);
            }
        }

        private void UpdateFlights(FlightSearch flightSearch, short paxCount)
        {
            foreach (var flight in flightSearch.Flights)
            {
                var newFlight = _flightService.Get(flightSearch.DepartureStation,
                                                   flightSearch.ArrivalStation, flight.DepartureDate,
                                                   flight.CarrierCode,
                                                   (short)flight.FlightNumber, paxCount);

                if (newFlight != null)
                {
                    flight.Price = (int)(newFlight.Price * 100);
                }
                else
                {
                    // todo: disable flight with remark!
                    flight.Price = 0;
                }
            }
        }
    }
}
