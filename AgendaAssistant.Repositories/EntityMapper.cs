using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Entities;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Repositories
{
    public static class EntityMapper
    {
        public static Event Map(DB.Event dbEvent)
        {
            var evn = new Event()
                {
                    EventId = dbEvent.ID,
                    Code = CodeString.GuidAsCodeString(dbEvent.EventID),
                    Description = dbEvent.Description,
                    Status = dbEvent.Status,
                    Title = dbEvent.Title,
                    IsConfirmed = dbEvent.IsConfirmed,
                    Organizer = Map(dbEvent.Organizer),
                    Participants = new List<Participant>(),
                    OutboundFlightSearch = Map(dbEvent.OutboundFlightSearch),
                    InboundFlightSearch = Map(dbEvent.InboundFlightSearch)
                };

            dbEvent.Participants.ToList()
                   .ForEach(p => evn.Participants.Add(new Participant() { PersonId = p.ID, Name = p.Name, Email = p.Email}));

            return evn;
        }

        public static Person Map(DB.Person dbPerson)
        {
            return new Person
                {
                    Id = dbPerson.ID,
                    Name = dbPerson.Name,
                    Email = dbPerson.Email
                };
        }

        public static FlightSearch Map(DB.FlightSearch dbFlightSearch)
        {
            var flightSearch = new FlightSearch
                {
                    Id = dbFlightSearch.ID,
                    DepartureStation = dbFlightSearch.DepartureStation,
                    ArrivalStation = dbFlightSearch.ArrivalStation,
                    BeginDate = dbFlightSearch.StartDate,
                    EndDate = dbFlightSearch.EndDate,
                    Flights = new List<Flight>()
                };

            dbFlightSearch.Flights.ToList().ForEach(f => flightSearch.Flights.Add(Map(f)));

            return flightSearch;
        }

        public static Flight Map(DB.Flight dbFlight)
        {
            return new Flight
                {
                    Id = dbFlight.ID,
                    CarrierCode = dbFlight.CarrierCode,
                    FlightNumber = (short) dbFlight.FlightNumber,
                    ArrivalStation = dbFlight.FlightSearch.ArrivalStation,
                    DepartureStation = dbFlight.FlightSearch.DepartureStation,
                    DepartureDate = dbFlight.DepartureDate,
                    STA = dbFlight.STA,
                    STD = dbFlight.STD,
                    Price = dbFlight.Price / 100M
                };
        }

        public static Availability Map(DB.Availability dbAvailability)
        {
            return new Availability
            {
                Value = dbAvailability.Value,
                CommentText = dbAvailability.Comment
            };
        }
    }
}