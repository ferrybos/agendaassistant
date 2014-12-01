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
        public static Event Map(DB.Event dbEvent, bool includeParticipants = true, bool includeFlights = true, bool includeAvailability = true)
        {
            var evn = new Event()
                {
                    Id = GuidUtil.ToString(dbEvent.ID),
                    Description = dbEvent.Description,
                    //Status = Map(dbEvent.EventStatus),
                    Title = dbEvent.Title,
                    Pnr = dbEvent.PNR,
                    PushpinCompleted = dbEvent.StatusID >= EventStatusEnum.PushpinCompleted,

                    IsConfirmed = dbEvent.StatusID >= EventStatusEnum.Confirmed,
                    OrganizerName = dbEvent.OrganizerName,
                    OrganizerEmail = dbEvent.OrganizerEmail,

                    Participants = new List<Participant>()
                };

            if (includeParticipants)
            {
                dbEvent.Participants.ToList().ForEach(p => evn.Participants.Add(Map(p)));
            }

            if (includeFlights)
            {
                evn.OutboundFlightSearch = Map(dbEvent.OutboundFlightSearch);
                evn.InboundFlightSearch = Map(dbEvent.InboundFlightSearch);
               
                evn.OutboundFlights = new List<Flight>();
                evn.InboundFlights = new List<Flight>();
            }

            if (includeAvailability && evn.IsConfirmed) // confirmed event has flights and participants
            {
                var flightsDictionary = new Dictionary<long, Flight>();
                evn.OutboundFlightSearch.Flights.ForEach(f => flightsDictionary.Add(f.Id, f));
                evn.InboundFlightSearch.Flights.ForEach(f => flightsDictionary.Add(f.Id, f));

                foreach (var dbParticipant in dbEvent.Participants.Where(p => p.AvailabilityConfirmed))
                {
                    foreach (var dbAvailability in dbParticipant.Availabilities)
                    {
                        flightsDictionary[dbAvailability.FlightID].Availabilities.Add(Map(dbAvailability, dbParticipant.Name));
                    }
                }
            }

            return evn;
        }

        //private static EventStatus Map(DB.EventStatus eventStatus)
        //{
        //    return new EventStatus() { Id = eventStatus.ID, Description = eventStatus.Description };
        //}

        public static FlightSearch Map(DB.FlightSearch dbFlightSearch)
        {
            if (dbFlightSearch == null)
                return null;

            var flightSearch = new FlightSearch
                {
                    Id = dbFlightSearch.ID,
                    DepartureStation = dbFlightSearch.DepartureStation.Trim(),
                    ArrivalStation = dbFlightSearch.ArrivalStation.Trim(),
                    BeginDate = dbFlightSearch.StartDate,
                    EndDate = dbFlightSearch.EndDate,
                    DaysOfWeek = (short)dbFlightSearch.DaysOfWeek,
                    MaxPrice = (short?)dbFlightSearch.MaxPrice,
                    Flights = new List<Flight>()
                };

            dbFlightSearch.Flights.ToList().ForEach(f => flightSearch.Flights.Add(Map(f)));

            // link selected flight from collection
            if (dbFlightSearch.SelectedFlightID.HasValue)
            {
                flightSearch.SelectedFlight =
                    flightSearch.Flights.Single(f => f.Id.Equals(dbFlightSearch.SelectedFlightID));
            }

            return flightSearch;
        }

        public static Flight Map(DB.Flight dbFlight)
        {
            return new Flight
                {
                    Id = dbFlight.ID,
                    CarrierCode = dbFlight.CarrierCode,
                    FlightNumber = (short)dbFlight.FlightNumber,
                    ArrivalStation = dbFlight.FlightSearch.ArrivalStation.Trim(),
                    DepartureStation = dbFlight.FlightSearch.DepartureStation.Trim(),
                    DepartureDate = dbFlight.DepartureDate,
                    STA = dbFlight.STA,
                    STD = dbFlight.STD,
                    Price = dbFlight.Price / 100M,
                    Enabled = dbFlight.Enabled
                };
        }

        public static Availability Map(DB.Availability dbAvailability, string name)
        {
            return new Availability
            {
                ParticipantId = GuidUtil.ToString(dbAvailability.ParticipantID),
                FlightId = dbAvailability.FlightID,
                Value = dbAvailability.Value ?? 0,
                CommentText = dbAvailability.Comment.Trim(),
                Name = name
            };
        }

        public static Participant Map(DB.Participant dbParticipant)
        {
            var participant = new Participant
            {
                Id = GuidUtil.ToString(dbParticipant.ID),

                EventId = GuidUtil.ToString(dbParticipant.EventID),
                Person = new Person()
                {
                    Name = dbParticipant.Name,
                    Email = dbParticipant.Email,
                    FirstNameInPassport = dbParticipant.FirstNameInPassport,
                    LastNameInPassport = dbParticipant.LastNameInPassport,
                    DateOfBirth = dbParticipant.DateOfBirth,
                    Gender = dbParticipant.Gender.HasValue ? (Gender)dbParticipant.Gender.Value : (Gender?)null
                },
                Bagage = dbParticipant.Bagage.Trim(),
                AvailabilityConfirmed = dbParticipant.AvailabilityConfirmed,
                BookingDetailsConfirmed = dbParticipant.BookingDetailsConfirmed
            };

            return participant;
        }

        public static Station Map(DB.Station dbStation)
        {
            return new Station() { Code = dbStation.Code, Name = dbStation.Name };
        }

        public static Route Map(DB.Route dbRoute)
        {
            return new Route() { Origin = dbRoute.Origin, Destination = dbRoute.Destination };
        }
    }
}