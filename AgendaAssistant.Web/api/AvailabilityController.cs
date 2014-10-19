using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AgendaAssistant.Entities;
using AgendaAssistant.Repositories;
using AgendaAssistant.Services;
using AgendaAssistant.Web.models;

namespace AgendaAssistant.Web.api
{
    [RoutePrefix("api/availability")]
    public class AvailabilityController : ApiController
    {
        private readonly IAvailabilityService _availabilityService;
        private readonly IEventService _eventService;

        public AvailabilityController(IAvailabilityService availabilityService, IEventService eventService)
        {
            _availabilityService = availabilityService;
            _eventService = eventService;
        }

        // GET api/<controller>/5
        [Route("{eventId}/{personId}")]
        public AvailabilityModel Get(string eventId, long personId)
        {           
            var evn = _eventService.Get(eventId);

            var outboundAvailabilities = _availabilityService.Get(evn.OutboundFlightSearch.Id, personId);
            foreach (var flight in evn.OutboundFlightSearch.Flights)
            {
                flight.Availabilities = new List<Availability>();
                flight.Availabilities.AddRange(outboundAvailabilities.Where(a => a.FlightId == flight.Id));
            }

            var inboundAvailabilities = _availabilityService.Get(evn.InboundFlightSearch.Id, personId);
            foreach (var flight in evn.InboundFlightSearch.Flights)
            {
                flight.Availabilities = new List<Availability>();
                flight.Availabilities.AddRange(inboundAvailabilities.Where(a => a.FlightId == flight.Id));
            }

            // fetch existing event
            return new AvailabilityModel()
                {
                    Event = evn
                };
        }

        [Route("")]
        [HttpPost]
        public IHttpActionResult Post([FromBody] Availability availability)
        {
            // update av
            _availabilityService.Update(availability);
            return Ok();
        }
    }
}