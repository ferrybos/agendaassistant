using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaAssistant.Entities
{
    public class Flight
    {
        public long Id { get; set; }

        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime STD { get; set; }
        public DateTime STA { get; set; }
        public string CarrierCode { get; set; }
        public short FlightNumber { get; set; }
        public decimal Price { get; set; }

        public List<Availability> Availabilities { get; set; }
    }
}
