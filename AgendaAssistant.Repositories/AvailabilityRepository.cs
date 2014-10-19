using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.DB;
using Availability = AgendaAssistant.Entities.Availability;
using Event = AgendaAssistant.Entities.Event;
using FlightSearch = AgendaAssistant.Entities.FlightSearch;
using Person = AgendaAssistant.Entities.Person;

namespace AgendaAssistant.Repositories
{
    public interface IAvailabilityRepository
    {
        List<Availability> Get(long flightSearchId, long personId);
        void Update(Availability availability);
    }

    /// <summary>
    /// Contains all logic to interface with data(base)
    /// </summary>
    public class AvailabilityRepository : IAvailabilityRepository
    {
        private readonly AgendaAssistantEntities _db;

        public AvailabilityRepository()
        {
            _db = DbContextFactory.New();
        }

        public void Update(Availability availability)
        {
            var dbAvailability =
                _db.Availabilities.Single(
                    a => a.FlightID == availability.FlightId && a.PersonID == availability.PersonId);

            dbAvailability.Value = availability.Value;
            dbAvailability.Comment = availability.CommentText;

            _db.SaveChanges();
        }

        public List<Availability> Get(long flightSearchId, long personId)
        {
            var result = new List<Availability>();

            var dbFlightSearch = _db.FlightSearches.Single(fs => fs.ID == flightSearchId);

            var dbAvailabilities =
                _db.Availabilities.Where(a => a.Flight.FlightSearchID == flightSearchId && a.PersonID == personId)
                   .ToList();

            foreach (var dbFlight in dbFlightSearch.Flights)
            {
                var dbAvailability = dbAvailabilities.SingleOrDefault(a => a.FlightID == dbFlight.ID);

                if (dbAvailability == null)
                {
                    // does not exist yet, create new
                    dbAvailability = _db.Availabilities.Create();
                    _db.Availabilities.Add(dbAvailability);

                    dbAvailability.FlightID = dbFlight.ID;
                    dbAvailability.PersonID = personId;
                    dbAvailability.Value = 0;

                    dbAvailabilities.Add(dbAvailability);
                }

                var availability = EntityMapper.Map(dbAvailability);
                availability.FlightId = dbFlight.ID;
                availability.PersonId = personId;
                result.Add(availability);
            }

            _db.SaveChanges();

            return result;
        }
    }
}
