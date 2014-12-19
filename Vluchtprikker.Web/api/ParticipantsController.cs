using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vluchtprikker.Entities;
using Vluchtprikker.Services;
using Vluchtprikker.Shared;

namespace Vluchtprikker.Web.api
{
    public class ParticipantData
    {
        public string EventCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class UpdatePersonData
    {
        public string ParticipantId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool SendInvitation { get; set; }
    }

    [RoutePrefix("api/participants")]
    public class ParticipantsController : ApiBaseController
    {
        private readonly IParticipantService _service;

        public ParticipantsController(IParticipantService participantService)
        {
            _service = participantService;
        }

        // Required field should be included in the route
        // Optional fields should be added as query string parameters, for example max price
        [Route("{participantId}")]
        [HttpGet]
        public IHttpActionResult Get(string participantId)
        {
            try
            {
                var participant = _service.Get(participantId);
                return Ok(participant);
            }
            catch (Exception ex)
            {
                HandleServerError(ex);
                return InternalServerError();
            }
        }

        /// <summary>
        /// Called to add new participants
        /// </summary>
        [Route("")]
        [HttpPost]
        public IHttpActionResult Post([FromBody] Participant participant)
        {
            // new participant
            try
            {
                var newParticipant = _service.Add(participant.EventId, participant.Person.Name, participant.Person.Email);

                return Created(string.Format("api/participants/{0}", newParticipant.Id), Json(newParticipant).Content);
            }
            catch (Exception ex)
            {
                HandleServerError(ex);
                return InternalServerError();
            }
        }

        /// <summary>
        /// Called to update booking data
        /// </summary>
        [Route("")]
        [HttpPut]
        public IHttpActionResult Put([FromBody] Participant participant)
        {
            // update participant
            try
            {
                _service.Update(participant);
                return Ok();
            }
            catch (Exception ex)
            {
                HandleServerError(ex);
                return InternalServerError(ex); // return error details
            }
        }

        /// <summary>
        /// Called to update booking data
        /// </summary>
        [Route("updatePerson")]
        [HttpPost]
        public IHttpActionResult UpdatePerson([FromBody] UpdatePersonData data)
        {
            // update participant
            try
            {
                _service.UpdatePerson(data.ParticipantId, data.Name, data.Email, data.SendInvitation);
                return Ok();
            }
            catch (Exception ex)
            {
                HandleServerError(ex);
                return InternalServerError();
            }
        }
        /// <summary>
        /// Called to delete participants
        /// </summary>
        [Route("{id}")]
        [HttpDelete]
        public IHttpActionResult Delete(string id)
        {
            // delete participant
            try
            {
                _service.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                HandleServerError(ex);
                return InternalServerError();
            }
        }
    }
}