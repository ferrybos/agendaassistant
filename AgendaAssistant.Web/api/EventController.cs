using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AgendaAssistant.Entities;

namespace AgendaAssistant.Web.api
{
    [RoutePrefix("api/event")]
    public class EventController : ApiController
    {
        // GET api/<controller>/5
        [Route("{id}")]
        public Event Get(int id)
        {
            if (id == 0)
            {
                // new event
                var newEvent = EventFactory.NewEvent();
                newEvent.Title = "Weekendje Barcelona";
                newEvent.Description = "Dit is een test";
                newEvent.Organizer.Name = "Ferry Bos";
                newEvent.Organizer.Email = "ferry.bos@transavia.com";
                return newEvent;
            }
            else
            {
                // fetch existing event
                throw new NotImplementedException();
            }
        }

        // POST api/<controller>
        public void Post([FromBody]Event value)
        {
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