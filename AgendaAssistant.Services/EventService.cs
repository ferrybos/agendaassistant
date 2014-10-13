using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Entities;
using AgendaAssistant.Repositories;

namespace AgendaAssistant.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _repository;

        public EventService(IEventRepository repository)
        {
            _repository = repository;
        }

        public Event CreateNew(Event value)
        {
            return _repository.CreateNew(value);
        }
    }

    public interface IEventService
    {
        Event CreateNew(Event value);
    }
}
