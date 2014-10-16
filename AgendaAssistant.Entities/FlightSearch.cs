using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaAssistant.Entities
{
    public class FlightSearch
    {
        public long Id { get; set; }

        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<Flight> Flights { get; set; }
    }
}
