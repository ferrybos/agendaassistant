using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaAssistant.Web.models
{
    public class ParticipantViewModel
    {
        public Event Event;
        public Stretch Outbound;
        public Stretch Inbound;
        public List<Availability> Availabilities;
    }
}
