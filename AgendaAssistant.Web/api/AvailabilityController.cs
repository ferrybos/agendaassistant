using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vluchtprikker.Repositories;
using Vluchtprikker.Services;
using Vluchtprikker.Shared;
using Vluchtprikker.Entities;

namespace Vluchtprikker.Web.api
{
    public class ParticipantId
    {
        public string Id { get; set; }
    }

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

        /// <summary>
        /// Called to add new participants
        /// </summary>
        [Route("confirm")]
        [HttpPost]
        public IHttpActionResult Confirm([FromBody] ParticipantId data)
        {
            // new participant
            try
            {
                _service.Confirm(data.Id);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}