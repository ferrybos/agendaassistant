using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaAssistant.Web.models
{
    public class Stretch
    {
        public Station Origin;
        public Station Destination;
        public List<Flight> Flights;
        public long SelectedFlightId;
    }
}
