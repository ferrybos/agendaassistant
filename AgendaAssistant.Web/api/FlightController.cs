using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AgendaAssistant.Entities;
using AgendaAssistant.Services;

namespace AgendaAssistant.Web.api
{
    [RoutePrefix("api/flight")]
    public class FlightController : ApiController
    {
        private readonly IFlightService _service;

        public FlightController(IFlightService flightService)
        {
            _service = flightService;
        }

        // Required field should be included in the route
        // Optional fields should be added as query string parameters, for example max price
        [Route("{departureStation}/{arrivalStation}/{beginDate:DateTime}/{endDate:DateTime}/{paxCount}/{daysOfWeek}")]
        [HttpGet]
        public List<Flight> Get(string departureStation, string arrivalStation, DateTime beginDate, DateTime endDate, short paxCount, short daysOfWeek, short? maxPrice = null)
        {
            var bitArray = new BitArray(BitConverter.GetBytes(daysOfWeek));
            return _service.Search(departureStation, arrivalStation, beginDate, endDate, paxCount, bitArray, maxPrice);
        }
    }
}