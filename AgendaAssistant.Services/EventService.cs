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
        private readonly IMailService _mailService;

        public EventService(IEventRepository repository, IMailService mailService)
        {
            _repository = repository;
            _mailService = mailService;
        }

        public Event CreateNew(Event value)
        {
            var evn = _repository.Save(value);

            _mailService.SendEventConfirmation(evn);
            
            return evn;
        }

        public Event Get(string code)
        {
            return _repository.Get(code);
        }

        public void Confirm(string code)
        {
            _repository.Confirm(code);

            var evn = Get(code);
            
            foreach (var participant in evn.Participants)
                _mailService.SendInvitation(evn, participant);

            _mailService.SendInvitationConfirmation(evn);
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
