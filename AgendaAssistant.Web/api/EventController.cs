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
        public Event Get(int id)
        {
            if (id == 0)
            {
                // new event
                var newEvent = EventFactory.NewEvent();
                return newEvent;
            }
            else
            {
                // fetch existing event
                throw new NotImplementedException();
            }
        }

        // POST api/<controller>
        [Route("")]
        [HttpPost]
        public void Post([FromBody]Event value)
        {
            // create new event
            _service.CreateNew(value);
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]Event value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}