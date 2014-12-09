using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vluchtprikker.Entities
{
    public static class FlightSearchResponseFactory
    {
        public static FlightSearchResponse New()
        {
            return new FlightSearchResponse()
                {
                    OutboundFlights = new List<Flight>(),
                    InboundFlights = new List<Flight>()
                };
        }
    }

    public class FlightSearchResponse
    {
        public List<Flight> OutboundFlights { get; set; }
        public List<Flight> InboundFlights { get; set; }
    }
}
