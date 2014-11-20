using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Entities
{
    public class EventStatus
    {
        public short Id { get; set; }
        public string Description { get; set; }
    }

    public class Event
    {
        public string Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Pnr { get; set; }
        public bool PushpinCompleted { get; set; }
        
        public EventStatus Status { get; set; }
        public bool IsConfirmed { get; set; }

        public string OrganizerName { get; set; }
        public string OrganizerEmail { get; set; }

        public List<Participant> Participants { get; set; }
        public FlightSearch OutboundFlightSearch { get; set; }
        public FlightSearch InboundFlightSearch { get; set; }

        public List<Availability> Availabilities()
        {
            var result = new List<Availability>();

            OutboundFlightSearch.Flights.ForEach(f => result.AddRange(f.Availabilities));
            InboundFlightSearch.Flights.ForEach(f => result.AddRange(f.Availabilities));

            return result;
        }
    }
}
