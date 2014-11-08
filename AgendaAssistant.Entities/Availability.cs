using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaAssistant.Entities
{
    public class Availability
    {
        public Guid ParticipantId { get; set; }
        public long FlightId { get; set; }

        public short Value { get; set; }
        public string CommentText { get; set; }

        // Display properties
        public string Name { get; set; }
    }
}
