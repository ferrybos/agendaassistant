using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Entities
{
    public class Event
    {
        public string Id { get; set; }

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
