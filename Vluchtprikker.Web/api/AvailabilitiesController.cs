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

    [RoutePrefix("api/availabilities")]
    public class AvailabilitiesController : ApiBaseController
    {
        private readonly IAvailabilityService _service;

        public AvailabilitiesController(IAvailabilityService availabilityService)
        {
            _service = availabilityService;
        }

        [Route("{participantId}")]
        [HttpGet]
        public IHttpActionResult Get(string participantId)
        {
            try
            {
                var evn = _service.Get(participantId);
                return Ok(evn);
            }
            catch (Exception ex)
            {
                HandleServerError(ex);
                return InternalServerError(ex);
            }
        }

        [Route("")]
        [HttpPost]
        public IHttpActionResult Post([FromBody] Availability availability)
        {
            try
            {
                _service.Update(availability);
                return Ok();
            }
            catch (Exception ex)
            {
                HandleServerError(ex);
                return InternalServerError(ex);
            }
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
                HandleServerError(ex);
                return InternalServerError(ex);
            }
        }
    }
}