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
        Participant Get(string id);
        Participant Add(string eventId, string name, string email);
        void Update(Participant participant);
        void Delete(string id);
    }

    public class ParticipantService : IParticipantService
    {
        private readonly IDbContext _dbContext;
        private readonly ParticipantRepository _repository;
        private readonly PersonRepository _personRepository;
        private readonly IMailService _mailService;

        public ParticipantService(IDbContext dbContext, IMailService mailService)
        {
            _dbContext = dbContext;

            _repository = new ParticipantRepository(_dbContext);
            _personRepository = new PersonRepository(_dbContext);
            _mailService = mailService;
        }

        public Participant Get(string id)
        {
            var dbParticipant = _repository.Single(GuidUtil.ToGuid(id));

            return EntityMapper.Map(dbParticipant);
        }

        public Participant Add(string eventId, string name, string email)
        {
            var dbEvent = new EventRepository(_dbContext).Single(GuidUtil.ToGuid(eventId));
            var dbPerson = _personRepository.AddOrGetExisting(name, email);
            
            var dbParticipant = _repository.Add(dbEvent.ID, dbPerson.ID);
            
            return EntityMapper.Map(dbParticipant);
        }

        public void Update(Participant participant)
        {
            var dbParticipant = _repository.Update(GuidUtil.ToGuid(participant.Id), participant.Bagage);
            
            var person = participant.Person;
            _personRepository.Update(GuidUtil.ToGuid(person.Id),
                                     person.FirstNameInPassport, person.LastNameInPassport,
                                     person.DateOfBirth, person.Gender);

            _mailService.SendBookingDetails(dbParticipant);

            dbParticipant.BookingDetailsConfirmed = true;
            _dbContext.Current.SaveChanges();
        }

        public void Delete(string id)
        {
            _repository.Delete(GuidUtil.ToGuid(id));
        }
    }
}
