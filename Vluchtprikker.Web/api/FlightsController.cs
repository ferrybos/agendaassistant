using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vluchtprikker.Entities;
using Vluchtprikker.Repositories;
using Vluchtprikker.Services;

namespace Vluchtprikker.Web.api
{
    [RoutePrefix("api/flights")]
    public class FlightsController : ApiBaseController
    {
        private readonly IFlightRepository _repository;

        public FlightsController(IFlightRepository repository)
        {
            _repository = repository;
        }

        // Required field should be included in the route
        // Optional fields should be added as query string parameters, for example max price
        [Route("{departureStation}/{arrivalStation}/{beginDate:DateTime}/{endDate:DateTime}/{paxCount}/{daysOfWeek}")]
        [HttpGet]
        public FlightSearchResponse Get(string departureStation, string arrivalStation, DateTime beginDate, DateTime endDate, short paxCount, short daysOfWeek, short? maxPrice = null)
        {
            var bitArray = new BitArray(BitConverter.GetBytes(daysOfWeek));
            return _repository.Search(departureStation, arrivalStation, beginDate, endDate, paxCount, bitArray, maxPrice);
        }
    }
}