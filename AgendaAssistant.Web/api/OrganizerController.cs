using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AgendaAssistant.Web.models;

namespace AgendaAssistant.Web.api
{
    [RoutePrefix("api/organizer")]
    public class OrganizerController : ApiController
    {
        public OrganizerController()
        {
            
        }

        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            try
            {
                var model = new OrganizerEventViewModel();

                // todo: fetch event with given id (IEventRepository)
                model.Event = new Event();
                
                // todo: fetch participants
                //model.Participants = new Participants();

                // todo: fetch flights
                model.Outbound = new Stretch();
                model.Inbound = new Stretch();

                // todo: fetch availability (flightId)
                //model.Availabilities = 

                //Json(new { Event = newEvent }).Content);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
