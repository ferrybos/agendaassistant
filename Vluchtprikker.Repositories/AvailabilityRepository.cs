﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vluchtprikker.DB;
using Vluchtprikker.DB.Repositories;
using Vluchtprikker.Shared;

namespace Vluchtprikker.Repositories
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

        public Availability Create(Participant dbParticipant, Flight dbFlight, short? value = null)
        {

            var dbAvailability = DbContext.Availabilities.Create();
            dbParticipant.Availabilities.Add(dbAvailability);

            dbAvailability.FlightID = dbFlight.ID;
            dbAvailability.Comment = "";
            dbAvailability.Value = value;

            return dbAvailability;
        }

        //public Availability Create(Guid participantId, long flightId)
        //{
        //    var dbAvailability = DbContext.Availabilities.Create();
        //    DbContext.Availabilities.Add(dbAvailability);

        //    dbAvailability.Comment = "";
        //    dbAvailability.ParticipantID = participantId;
        //    dbAvailability.FlightID = flightId;
        //    dbAvailability.Value = null;

        //    DbContext.SaveChanges();

        //    return dbAvailability;
        //}

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

        public void Update(Guid participantId, long flightId, short? value, string comment)
        {
            var dbAvailability = Single(participantId, flightId);

            dbAvailability.Value = value;
            dbAvailability.Comment = comment;

            DbContext.SaveChanges();
        }
    }
}
