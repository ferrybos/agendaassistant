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
    /// <summary>
    /// Contains all logic to interface with data(base)
    /// </summary>
    public class AvailabilityRepository : DbRepository
    {
        public AvailabilityRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        public Availability Single(Guid participantId, long flightId)
        {
            var dbAvailability =
                DbContext.Availabilities.Single(
                    e => e.ParticipantID.Equals(participantId) && e.FlightID.Equals(flightId));

            if (dbAvailability == null)
            {
                throw new ApplicationException(
                    string.Format("Availability not found with participantId {0} and flightId {1}", participantId,
                                  flightId));
            }

            return dbAvailability;
        }

        public List<Availability> SelectAll(long flightSearchId)
        {
            return DbContext.Availabilities.Where(a => a.Flight.FlightSearchID == flightSearchId).ToList();
        }

        public List<Availability> SelectAll(Guid participantId)
        {
            return
                DbContext.Availabilities.Where(a => a.ParticipantID.Equals(participantId)).ToList();
        }

        public void Update(Guid participantId, long flightId, short value, string comment)
        {
            var dbAvailability = Single(participantId, flightId);

            dbAvailability.Value = value;
            dbAvailability.Comment = comment;

            DbContext.SaveChanges();
        }
    }
}
