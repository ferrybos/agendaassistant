using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AgendaAssistant.Entities;

namespace AgendaAssistant.Web.api
{
    public class FlightController : ApiController
    {
        public List<Flight> Get()
        {
            var result = new List<Flight>();

            result.Add(new Flight() { DepartureStation = "AMS", ArrivalStation = "BCN", CarrierCode = "HV", FlightNumber = 5131, STD = DateTime.Now, STA = DateTime.Now.AddHours(1), Price = 100});
            result.Add(new Flight() { DepartureStation = "AMS", ArrivalStation = "BCN", CarrierCode = "HV", FlightNumber = 5135, STD = DateTime.Now.AddHours(5), STA = DateTime.Now.AddHours(6), Price = 120 });

            return result;
        }

        // GET api/<controller>/5
        public List<Flight> Get(string departureStation, string arrivalStation, DateTime beginDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }
}