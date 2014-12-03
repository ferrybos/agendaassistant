using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaAssistant.Web.models
{
    public class NewEventViewModel
    {
        public Event Event;
        public List<Person> Participants;
        public FlightSearch FlightSearch; // not needed in this viewmodel!
        public Stretch Outbound;
        public Stretch Inbound;
    }
}
