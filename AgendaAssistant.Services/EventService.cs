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
        Event Get(Guid id);
        Event Create(string title, string description, string name, string email);

        void Complete(Event evn);

        void SelectFlight(Guid id, long flightSearchId, long flightId);

        void Confirm(Guid id);
    }

    /// <summary>
    /// Contains all business logic regarding events
    /// </summary>
    public class EventService : IEventService
    {
        private readonly IMailService _mailService;
        private readonly EventRepository _repository;
        private readonly IDbContext _dbContext;

        public EventService(IDbContext dbContext, IMailService mailService)
        {
            _dbContext = dbContext;
            _repository = new EventRepository(_dbContext);
            _mailService = mailService;
        }

        public Event Get(Guid id)
        {
            // fetch event (including complete tree)
            var dbEvent = _repository.Single(id);

            return EntityMapper.Map(dbEvent);
        }

        public Event Create(string title, string description, string name, string email)
        {
            var dbOrganizerPerson = new PersonRepository(_dbContext).AddOrGetExisting(name, email);
            var dbEvent = _repository.Create(title, description, dbOrganizerPerson);

            // by default, add organizer as participant
            new ParticipantRepository(_dbContext).Add(dbEvent.ID, dbOrganizerPerson.ID);

            // todo: _mailService.SendEventConfirmation(evn);

            return EntityMapper.Map(dbEvent);
        }

        public void Confirm(Guid id)
        {
            var dbEvent = _repository.Confirm(id);

            // todo:
            //foreach (var participant in dbEvent.Participants)
            //    _mailService.SendInvitation(evn, participant);

            //_mailService.SendInvitationConfirmation(evn);
        }

        public void Complete(Event evn)
        {
            // todo: insert flightsearch
            var dbEvent = _repository.Single(evn.Id);

            dbEvent.OutboundFlightSearch = AddFlightSearch(evn.OutboundFlightSearch);
            dbEvent.InboundFlightSearch = AddFlightSearch(evn.InboundFlightSearch);

            _dbContext.Current.SaveChanges();
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

        public void SelectFlight(Guid id, long flightSearchId, long flightId)
        {
            //_repository.SelectFlight(flightSearchId, flightId);
        }
    }
}
