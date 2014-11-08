using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.DB;
using AgendaAssistant.DB.Repositories;
using AgendaAssistant.Extensions;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Repositories
{
    /// <summary>
    /// Contains all logic to interface with data(base)
    /// </summary>
    public class EventRepository : DbRepository
    {
        public EventRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        /// <summary>
        /// Fetches an event from the database with the given id 
        /// </summary>
        public Event Single(Guid id)
        {
            var dbEvent = DbContext.Events.SingleOrDefault(e => e.ID.Equals(id));

            if (dbEvent == null)
            {
                throw new ApplicationException(string.Format("Event not found with id {0}", id));
            }

            return dbEvent;
        }

        public Event Create(string title, string description, Person organizer)
        {
            var dbEvent = DbContext.Events.Create();
            DbContext.Events.Add(dbEvent);

            dbEvent.Title = title;
            dbEvent.Description = description;
            dbEvent.Organizer = organizer;

            dbEvent.ID = Guid.NewGuid(); // used as id for redirect links in app and emails
            dbEvent.CreatedUtc = DateTime.UtcNow;
            dbEvent.Status = "";
            dbEvent.IsConfirmed = false;

            DbContext.SaveChanges();

            return dbEvent;
        }

        public Event Confirm(Guid id)
        {
            var dbEvent = Single(id);

            if (!dbEvent.IsConfirmed)
            {
                dbEvent.IsConfirmed = true;
                dbEvent.Status = "Uitnodigingen verstuurd";

                DbContext.SaveChanges();
            }

            return dbEvent;
        }      

        //public void SelectFlight(long flightSearchId, long flightId)
        //{
        //    var dbFlightSearch = new DbFlightSearchRepository(_db).Get(flightSearchId);
        //    dbFlightSearch.SelectedFlightID = flightId;

        //    _db.SaveChanges();
        //}
    }
}
