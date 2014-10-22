using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaAssistant.Entities
{
    public class Availability
    {
        public short Value { get; set; }
        public string CommentText { get; set; }

        public long PersonId { get; set; }
        public long FlightId { get; set; }

        // Display properties
        public string Name { get; set; }
    }
}
