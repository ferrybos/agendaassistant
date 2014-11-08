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


            //// setup
            //var dbPersonRepository = new DbPersonRepository(DbContext);
            //var dbEventRepository = new DbEventRepository(DbContext);
            //var dbParticipantRepository = new DbParticipantRepository(DbContext);

            //// event and organizer
            //var organizerPerson = dbPersonRepository.AddPerson(value.Organizer.Name, value.Organizer.Email);
            //var dbEvent = dbEventRepository.AddEvent(value.Title, value.Description, organizerPerson, "", false);

            //// participants
            //foreach (var participant in value.Participants)
            //{
            //    var person = value.Organizer.Matches(participant.Name, participant.Email)
            //                        ? organizerPerson
            //                        : dbPersonRepository.AddPerson(participant.Name, participant.Email);

            //    dbParticipantRepository.AddParticipant(dbEvent, person, "");
            //}

            //// flights
            //dbEvent.OutboundFlightSearch = AddFlights(value.OutboundFlightSearch);
            //dbEvent.InboundFlightSearch = AddFlights(value.InboundFlightSearch);

            //// save event
            //DbContext.SaveChanges();

            //return EntityMapper.Map(dbEvent);
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

        //private DB.FlightSearch AddFlights(FlightSearch flightSearch)
        //{
        //    var dbFlightSearchRepository = new DbFlightSearchRepository(_db);
        //    var dbFlightRepository = new DbFlightRepository(_db);

        //    var dbFlightSearch = dbFlightSearchRepository.Create(flightSearch.ArrivalStation,
        //                                                  flightSearch.DepartureStation, flightSearch.BeginDate,
        //                                                  flightSearch.EndDate, flightSearch.DaysOfWeek, flightSearch.MaxPrice);

        //    foreach (var flight in flightSearch.Flights)
        //    {
        //        var dbFlight = dbFlightRepository.Create(flight.CarrierCode, flight.FlightNumber,
        //            flight.DepartureDate, flight.STD, flight.STA, (int)(flight.Price * 100));
        //        dbFlightSearch.Flights.Create(dbFlight);
        //    }

        //    return dbFlightSearch;
        //}

        

        //public void SelectFlight(long flightSearchId, long flightId)
        //{
        //    var dbFlightSearch = new DbFlightSearchRepository(_db).Get(flightSearchId);
        //    dbFlightSearch.SelectedFlightID = flightId;

        //    _db.SaveChanges();
        //}
    }
}
