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
    public interface IParticipantService
    {
        Participant Get(Guid id);
        Participant Add(Guid eventId, string name, string email);
        void Update(Participant participant);
        void Delete(Guid id);
    }

    public class ParticipantService : IParticipantService
    {
        private readonly IDbContext _dbContext;
        private readonly ParticipantRepository _repository;
        private readonly PersonRepository _personRepository;

        public ParticipantService(IDbContext dbContext)
        {
            _dbContext = dbContext;

            _repository = new ParticipantRepository(_dbContext);
            _personRepository = new PersonRepository(_dbContext);
        }

        public Participant Get(Guid id)
        {
            var dbParticipant = _repository.Single(id);

            return EntityMapper.Map(dbParticipant);
        }

        public Participant Add(Guid eventId, string name, string email)
        {
            var dbEvent = new EventRepository(_dbContext).Single(eventId);
            var dbPerson = _personRepository.AddOrGetExisting(name, email);
            
            var dbParticipant = _repository.Add(dbEvent.ID, dbPerson.ID);
            
            return EntityMapper.Map(dbParticipant);
        }

        public void Update(Participant participant)
        {
            _repository.Update(participant.Id, participant.Bagage);
            
            var person = participant.Person;
            _personRepository.Update(person.Id,
                                     person.FirstNameInPassport, person.LastNameInPassport,
                                     person.DateOfBirth, person.Gender);
        }

        public void Delete(Guid id)
        {
            _repository.Delete(id);
        }
    }
}
