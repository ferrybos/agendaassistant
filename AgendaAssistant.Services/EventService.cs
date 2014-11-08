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

        bool Confirm(Guid id);
    }

    /// <summary>
    /// Contains all business logic regarding events
    /// </summary>
    public class EventService : IEventService
    {
        private readonly EventRepository _repository;
        private readonly IDbContext _dbContext;

        public EventService(IDbContext dbContext)
        {
            _dbContext = dbContext;
            _repository = new EventRepository(_dbContext);
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

            return EntityMapper.Map(dbEvent);
        }

        public bool Confirm(Guid id)
        {
            return _repository.Confirm(id);
        }

        public void Complete(Event evn)
        {
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
