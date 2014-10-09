using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AgendaAssistant.Entities;

namespace AgendaAssistant.Web.api
{
    [RoutePrefix("api/flight")]
    public class FlightController : ApiController
    {
        // Required field should be included in the route
        // Optional fields should be added as query string parameters, for example carrier code or max price
        [Route("{departureStation}/{arrivalStation}/{beginDate:DateTime}/{endDate:DateTime}")]
        public List<Flight> Get(string departureStation, string arrivalStation, DateTime beginDate, DateTime endDate)
        {
            var result = new List<Flight>();

            var totalDays = (endDate - beginDate).TotalDays + 1; // including endDate
            for (int dayIndex = 0; dayIndex < totalDays; dayIndex++)
            {
                var date = beginDate.AddDays(dayIndex);
                result.Add(new Flight() { DepartureStation = departureStation, ArrivalStation = arrivalStation, CarrierCode = "HV", FlightNumber = 5131, STD = date.AddHours(1), STA = date.AddHours(2), Price = 100 });
                result.Add(new Flight() { DepartureStation = departureStation, ArrivalStation = arrivalStation, CarrierCode = "HV", FlightNumber = 5135, STD = date.AddHours(5), STA = date.AddHours(6), Price = 120 });
            }
                
            return result;
        }
    }
}