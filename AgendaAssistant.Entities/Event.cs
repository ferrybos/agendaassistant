using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Entities
{
    //public static class EventFactory
    //{
    //    public static Event NewEvent()
    //    {
    //        return new Event()
    //            {
    //                Code = 0,
    //                Organizer = PersonFactory.NewPerson(),
    //                Participants = new List<Participant>(),
    //                OutboundFlightSearch = new FlightSearch() {Flights = new List<Flight>()},
    //                InboundFlightSearch = new FlightSearch() {Flights = new List<Flight>()}
    //            };
    //    }
    //}

    public class Event
    {
        public Guid Id { get; set; }

        public string Code
        {
            get { return GuidUtil.ToString(Id); }
        }

        public string Title { get; set; }
        public string Description { get; set; }
        
        public string Status { get; set; }
        public bool IsConfirmed { get; set; }

        public string OrganizerParticipantCode { get; set; }

        public Person Organizer { get; set; }
        public List<Participant> Participants { get; set; }
        public FlightSearch OutboundFlightSearch { get; set; }
        public FlightSearch InboundFlightSearch { get; set; }

        public void AddAvailabilities(List<Availability> availabilities)
        {
            OutboundFlightSearch.AddAvailabilities(availabilities);
            InboundFlightSearch.AddAvailabilities(availabilities);
        }
    }
}
