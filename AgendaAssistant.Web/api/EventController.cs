using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AgendaAssistant.Entities;
using AgendaAssistant.Services;

namespace AgendaAssistant.Web.api
{
    [RoutePrefix("api/event")]
    public class EventController : ApiController
    {
        private readonly IEventService _service;

        public EventController(IEventService eventService)
        {
            _service = eventService;
        }

        // GET api/<controller>/5
        [Route("{id}")]
        public Event Get(string id)
        {
            if (id == "new")
            {
                // new event
                var newEvent = EventFactory.NewEvent();
                return newEvent;
            }
            
            // fetch existing event
            return _service.Get(id);
        }

        // POST api/<controller>
        [Route("")]
        [HttpPost]
        public IHttpActionResult Post([FromBody]Event value)
        {
            // create new event
            var createdEvent = _service.CreateNew(value);

            return Created(string.Format("api/event/{0}", createdEvent.EventId), Json(new { Code = createdEvent.Code }).Content);
        }

        [Route("confirm")]
        [HttpPost]
        public IHttpActionResult Confirm(string code)
        {
            try
            {
                _service.Confirm(code);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        // PUT api/<controller>/5
        //public void Put(int id, [FromBody]Event value)
        //{
        //}

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}