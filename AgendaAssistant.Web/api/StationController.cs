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
    public class StationsAndRoutesData
    {
        public List<Station> Origins { get; set; }
        public List<Station> Destinations { get; set; }
        public List<Route> Routes { get; set; }
    }

    [RoutePrefix("api/station")]
    public class StationController : ApiController
    {
        private readonly IStationService _service;

        public StationController(IStationService stationService)
        {
            _service = stationService;
        }

        // Required field should be included in the route
        // Optional fields should be added as query string parameters, for example max price
        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            var stations = _service.GetStations();
            var routes = _service.GetRoutes();

            var originCodes = routes.Select(r => r.Origin).Distinct().ToList();
            var destinationCodes = routes.Select(r => r.Destination).Distinct().ToList();

            var data = new StationsAndRoutesData
                {
                    Origins = stations.Where(s => originCodes.Contains(s.Code)).OrderBy(s => s.Name).ToList(),
                    Destinations = stations.Where(s => destinationCodes.Contains(s.Code)).OrderBy(s => s.Name).ToList(),
                    Routes = routes
                };

            return Ok(data);
        }
    }
}