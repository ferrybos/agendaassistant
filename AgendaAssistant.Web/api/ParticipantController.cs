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
    [RoutePrefix("api/participant")]
    public class ParticipantController : ApiController
    {
        private readonly IParticipantService _service;

        public ParticipantController(IParticipantService participantService)
        {
            _service = participantService;
        }

        // Required field should be included in the route
        // Optional fields should be added as query string parameters, for example max price
        [Route("{eventId}/{personId}")]
        public IHttpActionResult Get(string eventId, long personId)
        {
            try
            {
                var participant = _service.Get(eventId, personId);
                return Ok(participant);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("")]
        [HttpPost]
        public IHttpActionResult Post([FromBody] Participant participant)
        {
            // update participant
            try
            {
                _service.Update(participant);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}