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

        public AvailabilityController(IAvailabilityService availabilityService)
        {
            _service = availabilityService;
        }

        [Route("{participantId}")]
        [HttpGet]
        public IHttpActionResult Get(string participantId)
        {
            var evn = _service.Get(participantId);

            // fetch existing event
            return Ok(evn);
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