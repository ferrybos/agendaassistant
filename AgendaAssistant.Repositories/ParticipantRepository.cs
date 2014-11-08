using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.DB;
using AgendaAssistant.DB.Repositories;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Repositories
{
    public class ParticipantRepository : DbRepository
    {
        public ParticipantRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        /// <summary>
        /// Fetches an event from the database with the given id 
        /// </summary>
        public Participant Single(Guid id)
        {
            var dbParticipant = DbContext.Participants.SingleOrDefault(e => e.ID.Equals(id));

            if (dbParticipant == null)
            {
                throw new ApplicationException(string.Format("Participant not found with id {0}", id));
            }

            return dbParticipant;
        }

        public void Update(Guid id, string bagage)
        {
            var dbParticipant = Single(id);

            dbParticipant.Bagage = bagage;

            DbContext.SaveChanges();
        }

        public Participant Add(Guid eventId, Guid personId)
        {
            var dbParticipant = DbContext.Participants.Create();
            DbContext.Participants.Add(dbParticipant);

            dbParticipant.ID = Guid.NewGuid();
            dbParticipant.EventID = eventId;
            dbParticipant.PersonID = personId;
            dbParticipant.Bagage = "";

            DbContext.SaveChanges();

            return dbParticipant;
        }

        public void Delete(Guid id)
        {
            var dbParticipant = Single(id);
            DbContext.Participants.Remove(dbParticipant);

            DbContext.SaveChanges();
        }
    }
}
