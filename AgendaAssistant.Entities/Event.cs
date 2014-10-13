using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaAssistant.Entities
{
    public static class EventFactory
    {
        public static Event NewEvent()
        {
            return new Event()
                {
                    EventId = 0,
                    Organizer = PersonFactory.NewPerson(),
                    Participants = new List<Participant>(),
                    OutboundFlightSearch = new FlightSearch(),
                    InboundFlightSearch = new FlightSearch()
                };
        }
    }

    public class Event
    {
        public long EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Person Organizer { get; set; }
        public List<Participant> Participants { get; set; }
        public FlightSearch OutboundFlightSearch { get; set; }
        public FlightSearch InboundFlightSearch { get; set; }
    }

    public class Participant
    {
        public Person Person { get; set; }
    }
}
