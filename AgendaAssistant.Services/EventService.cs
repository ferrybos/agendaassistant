using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Entities;
using AgendaAssistant.Repositories;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Services
{
    /// <summary>
    /// Contains all business logic regarding events
    /// </summary>
    public class EventService : IEventService
    {
        private readonly IEventRepository _repository;

        public EventService(IEventRepository repository)
        {
            _repository = repository;
        }

        public Event CreateNew(Event value)
        {
            return _repository.Save(value);
        }

        public Event Get(string code)
        {
            return _repository.Get(code);
        }

        public void Confirm(string code)
        {
            _repository.Confirm(code);
        }

        public void SelectFlight(long flightSearchId, long flightId)
        {
            _repository.SelectFlight(flightSearchId, flightId);
        }
    }

    public interface IEventService
    {
        Event CreateNew(Event value);
        Event Get(string code);
        void Confirm(string code);
        void SelectFlight(long flightSearchId, long flightId);
    }
}
