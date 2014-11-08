using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Entities;
using AgendaAssistant.Repositories;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Services
{
    public interface IEventService
    {
        Event Get(Guid id);
        Event Create(string title, string description, string name, string email);

        void AddParticipant(Guid id, string name, string email);
        void DeleteParticipant(Guid id, string email);

        void Save(Event evn);

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

        public void AddParticipant(Guid id, string name, string email)
        {
            
        }

        public void DeleteParticipant(Guid id, string email)
        {
            throw new NotImplementedException();
        }

        public void Save(Event evn)
        {
            throw new NotImplementedException();
        }

        public void SelectFlight(Guid id, long flightSearchId, long flightId)
        {
            //_repository.SelectFlight(flightSearchId, flightId);
        }
    }
}
