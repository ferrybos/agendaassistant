using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vluchtprikker.Entities;
using Vluchtprikker.Repositories;
using Vluchtprikker.Shared;

namespace Vluchtprikker.Services
{
    public interface IParticipantService
    {
        Participant Get(string id);
        Participant Add(string eventId, string name, string email);
        void Update(Participant participant);
        void UpdatePerson(Participant participant);
        void Delete(string id);
    }

    public class ParticipantService : IParticipantService
    {
        private readonly IDbContext _dbContext;
        private readonly ParticipantRepository _repository;
        private readonly IMailService _mailService;

        public ParticipantService(IDbContext dbContext, IMailService mailService)
        {
            _dbContext = dbContext;

            _repository = new ParticipantRepository(_dbContext);
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
            
            var dbParticipant = _repository.Add(dbEvent.ID, name, email);
            
            return EntityMapper.Map(dbParticipant);
        }

        public void Update(Participant participant)
        {
            var gender = participant.Person.Gender.HasValue ? (byte)participant.Person.Gender : (byte?)null;
            var dbParticipant = _repository.Update(GuidUtil.ToGuid(participant.Id), participant.Bagage,
                                                   participant.Person.FirstNameInPassport,
                                                   participant.Person.LastNameInPassport, participant.Person.DateOfBirth,
                                                   gender);

            if (!dbParticipant.Gender.HasValue || string.IsNullOrWhiteSpace(dbParticipant.FirstNameInPassport) ||
                string.IsNullOrWhiteSpace(dbParticipant.LastNameInPassport) || dbParticipant.Bagage == null || !dbParticipant.DateOfBirth.HasValue)
            {
                throw new ApplicationException("U heeft nog niet alle boekingsgegevens ingevuld.");
            }

            var isComplete = dbParticipant.Availabilities.All(a => a.Value.HasValue);
            if (!isComplete)
            {
                throw new ApplicationException("U heeft nog niet alle beschikbaarheid ingevuld.");
            }

            var dbEvent = new EventRepository(_dbContext).Single(dbParticipant.EventID);

            // dont send to myself if the organizer is a participant also
            if (!dbParticipant.Email.Equals(dbEvent.OrganizerEmail))
                _mailService.SendAvailabilityUpdate(dbEvent, dbParticipant);

            dbParticipant.AvailabilityConfirmed = true;
            _dbContext.Current.SaveChanges();
        }

        public void UpdatePerson(Participant participant)
        {
            var dbParticipant = _repository.Single(GuidUtil.ToGuid(participant.Id));

            dbParticipant.Name = participant.Person.Name;
            dbParticipant.Email = participant.Person.Email;

            _dbContext.Current.SaveChanges();
        }

        public void Delete(string id)
        {
            _repository.Delete(GuidUtil.ToGuid(id));
        }
    }
}
