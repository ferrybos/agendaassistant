using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AgendaAssistant.Entities;
using AgendaAssistant.Repositories;
using AgendaAssistant.Services;
using AgendaAssistant.Shared;
using AgendaAssistant.Web.models;

namespace AgendaAssistant.Web.api
{
    [RoutePrefix("api/availability")]
    public class AvailabilityController : ApiController
    {
        private readonly IAvailabilityService _service;
        private readonly IEventService _eventService;

        public AvailabilityController(IAvailabilityService availabilityService, IEventService eventService)
        {
            _service = availabilityService;
            _eventService = eventService;
        }

        [Route("{eventId}/{participantId}")]
        [HttpGet]
        public AvailabilityModel Get(string eventId, string participantId)
        {
            // todo: no other participants
            var evn = _eventService.Get(GuidUtil.ToGuid(eventId));
            var availabilities = _service.GetByParticipant(GuidUtil.ToGuid(participantId));

            // map availabilities to flightsearch
            evn.OutboundFlightSearch.AddAvailabilities(availabilities);
            evn.InboundFlightSearch.AddAvailabilities(availabilities);

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
            _service.Update(availability);
            return Ok();
        }
    }
}