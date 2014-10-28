using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.DB;
using AgendaAssistant.DB.Repositories;
using AgendaAssistant.Entities;
using AgendaAssistant.Extensions;
using AutoMapper;
using Event = AgendaAssistant.Entities.Event;
using FlightSearch = AgendaAssistant.Entities.FlightSearch;
using Person = AgendaAssistant.Entities.Person;

namespace AgendaAssistant.Repositories
{
    public interface IEventRepository
    {
        Event Save(Event value);
        Event Get(string code);
        void Confirm(string code);
        void SelectFlight(long flightSearchId, long flightId);
    }

    /// <summary>
    /// Contains all logic to interface with data(base)
    /// </summary>
    public class EventRepository : IEventRepository
    {
        private readonly AgendaAssistantEntities _db;

        public EventRepository()
        {
            _db = DbContextFactory.New();
        }

        public Event Save(Event value)
        {
            // setup
            var dbPersonRepository = new DbPersonRepository(_db);
            var dbEventRepository = new DbEventRepository(_db);

            // event and organizer
            var organizerPerson = dbPersonRepository.AddPerson(value.Organizer.Name, value.Organizer.Email);
            var dbEvent = dbEventRepository.AddEvent(value.Title, value.Description, organizerPerson, "", false);

            // participants
            foreach (var participant in value.Participants)
            {
                var person = value.Organizer.Matches(participant.Name, participant.Email)
                                    ? organizerPerson
                                    : dbPersonRepository.AddPerson(participant.Name, participant.Email);

                dbEventRepository.AddParticipant(dbEvent, person);
            }

            // flights
            dbEvent.OutboundFlightSearch = AddFlights(value.OutboundFlightSearch);
            dbEvent.InboundFlightSearch = AddFlights(value.InboundFlightSearch);

            // save event
            _db.SaveChanges();

            return EntityMapper.Map(dbEvent);
        }

        private int BitArrayToInt(BitArray daysOfWeek)
        {
            short result = 0;

            for (int i = 0; i < daysOfWeek.Count; i++)
            {
                if (daysOfWeek[i])
                    result += Convert.ToInt16(Math.Pow(2, i));
            }

            return result;
        }

        private DB.FlightSearch AddFlights(FlightSearch flightSearch)
        {
            var dbFlightSearchRepository = new DbFlightSearchRepository(_db);
            var dbFlightRepository = new DbFlightRepository(_db);

            var dbFlightSearch = dbFlightSearchRepository.Add(flightSearch.ArrivalStation,
                                                          flightSearch.DepartureStation, flightSearch.BeginDate,
                                                          flightSearch.EndDate, flightSearch.DaysOfWeek, flightSearch.MaxPrice);

            foreach (var flight in flightSearch.Flights)
            {
                var dbFlight = dbFlightRepository.Add(flight.CarrierCode, flight.FlightNumber,
                    flight.DepartureDate, flight.STD, flight.STA, (int)(flight.Price * 100));
                dbFlightSearch.Flights.Add(dbFlight);
            }

            return dbFlightSearch;
        }

        public void Confirm(string code)
        {
            var dbEvent = GetDbEvent(code);

            if (!dbEvent.IsConfirmed)
            {
                dbEvent.IsConfirmed = true;
                dbEvent.Status = "Uitnodigingen verstuurd";

                _db.SaveChanges();
            }
        }

        public void SelectFlight(long flightSearchId, long flightId)
        {
            var dbFlightSearch = new DbFlightSearchRepository(_db).Get(flightSearchId);
            dbFlightSearch.SelectedFlightID = flightId;
            
            _db.SaveChanges();
        }

        public Event Get(string code)
        {
            var dbEvent = GetDbEvent(code);
            return EntityMapper.Map(dbEvent);
        }

        private DB.Event GetDbEvent(string code)
        {
            return new DbEventRepository(_db).Get(CodeString.CodeStringToGuid(code));
        }
    }
}
