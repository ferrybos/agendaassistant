using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.DB;
using AgendaAssistant.DB.Repositories;
using AgendaAssistant.Entities;
using Participant = AgendaAssistant.Entities.Participant;
using Person = AgendaAssistant.Entities.Person;

namespace AgendaAssistant.Repositories
{
    public interface IParticipantRepository
    {
        Participant Get(string eventCode, long personId);
        void Update(Participant participant);
    }

    public class ParticipantRepository : IParticipantRepository
    {
        private readonly AgendaAssistantEntities _db;

        public ParticipantRepository()
        {
            _db = DbContextFactory.New();
        }

        public Participant Get(string eventCode, long personId)
        {
            var dbEvent = new DbEventRepository(_db).Get(CodeString.CodeStringToGuid(eventCode));
            var dbParticipant = dbEvent.Participants.SingleOrDefault(p => p.PersonID == personId);

            return EntityMapper.Map(dbParticipant);
        }

        public void Update(Participant participant)
        {
            var dbParticipant =
                _db.Participants.Single(
                    p => p.EventID == participant.EventId && p.PersonID == participant.PersonId);

            var dbPerson = dbParticipant.Person;
            dbPerson.FirstNameInPassport = participant.Person.FirstNameInPassport;
            dbPerson.LastNameInPassport = participant.Person.LastNameInPassport;
            dbPerson.DateOfBirth = participant.Person.DateOfBirth;
            dbPerson.Gender = participant.Person.Gender.HasValue ? (byte) participant.Person.Gender : (byte?)null;
            
            dbParticipant.Baggage = participant.Baggage;

            _db.SaveChanges();
        }
    }
}
