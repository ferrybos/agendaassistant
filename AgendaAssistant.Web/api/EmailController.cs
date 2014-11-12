using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AgendaAssistant.Entities;
using AgendaAssistant.Repositories;
using AgendaAssistant.Services;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Web.api
{
    public class ParticipantEmailInput
    {
        public string ParticipantId { get; set; }
    }

    [RoutePrefix("api/email")]
    public class EmailController : ApiController
    {
        private readonly IMailService _service;

        public EmailController(IMailService mailService)
        {
            _service = mailService;
        }

        [Route("availability")]
        [HttpPost]
        public IHttpActionResult Availability([FromBody] ParticipantEmailInput data)
        {
            // new participant
            try
            {
                _service.SendAvailabilityUpdate(data.ParticipantId);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("bookingdetails")]
        [HttpPost]
        public IHttpActionResult BookingDetails([FromBody] ParticipantEmailInput data)
        {
            // new participant
            try
            {
                _service.SendBookingDetails(data.ParticipantId);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}