using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vluchtprikker.DB;
using Vluchtprikker.DB.Repositories;
using Vluchtprikker.Shared;

namespace Vluchtprikker.Repositories
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

        public Participant Update(Guid id, string bagage, string firstNameInPassport, string lastNameInPassport, DateTime? dateOfBirth, byte? gender)
        {
            var dbParticipant = Single(id);

            dbParticipant.Bagage = bagage;
            dbParticipant.FirstNameInPassport = firstNameInPassport;
            dbParticipant.LastNameInPassport = lastNameInPassport;
            dbParticipant.DateOfBirth = dateOfBirth;
            dbParticipant.Gender = gender;

            DbContext.SaveChanges();

            return dbParticipant;
        }

        public Participant Add(Guid eventId, string name, string email)
        {
            var dbParticipant = DbContext.Participants.Create();
            DbContext.Participants.Add(dbParticipant);

            dbParticipant.ID = Guid.NewGuid();
            dbParticipant.EventID = eventId;
            dbParticipant.Name = name;
            dbParticipant.Email = email;
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
