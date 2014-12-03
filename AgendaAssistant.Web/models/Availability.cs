using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaAssistant.Web.models
{
    public class Availability
    {
        public long FlightId;
        public string ParticipantName;
        public short? Value;
        public string Comment;
    }
}
