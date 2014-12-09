using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using Vluchtprikker.Entities;
using Vluchtprikker.Services;
using Vluchtprikker.Shared;

namespace Vluchtprikker.Web.api
{
    public class NewEventData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string OrganizerName { get; set; }
        public string OrganizerEmail { get; set; }
        public bool AddParticipant { get; set; }
    }

    public class InputData
    {
        public string Id { get; set; }
    }

    public class PnrData
    {
        public string Id { get; set; }
        public string Pnr { get; set; }
    }

    public class RefreshFlightsResponse
    {
        public List<Flight> OutboundFlights { get; set; }
        public List<Flight> InboundFlights { get; set; }
    }

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
        [HttpGet]
        public Event Get(string id)
        {
            var evn = _service.Get(id);

            return evn;
        }

        // POST api/<controller>
        [Route("")]
        [HttpPost]
        public IHttpActionResult Post(NewEventData data)
        {
            //System.Threading.Thread.Sleep(2000);

            // create new event
            try
            {
                var newEvent = _service.Create(data.Title, data.Description, data.OrganizerName, data.OrganizerEmail, data.AddParticipant);

                //Json(new { Event = newEvent }).Content);
                return Created(string.Format("api/event/{0}", newEvent.Id), Json(newEvent).Content);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            
        }

        [Route("confirm")]
        [HttpPost]
        public IHttpActionResult Confirm([FromBody]InputData data)
        {
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
        
        [Route("refreshflights")]
        [HttpPost]
        public IHttpActionResult RefreshFlights([FromBody]InputData data)
        {
            try
            {
                var updatedEvent = _service.RefreshFlights(data.Id);

                var response = new RefreshFlightsResponse()
                    {
                        OutboundFlights = updatedEvent.OutboundFlightSearch.Flights,
                        InboundFlights = updatedEvent.InboundFlightSearch.Flights
                    };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("selectflight")]
        [HttpPost]
        public IHttpActionResult SelectFlight(string id, long flightSearchId, long flightId)
        {
            try
            {
                _service.SelectFlight(id, flightSearchId, flightId);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT api/<controller>/5
        [Route("complete")]
        [HttpPost]
        public IHttpActionResult Complete([FromBody] Event value)
        {
            try
            {
                _service.Complete(value);

                return Ok(string.Format("api/event/{0}", value.Id));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("sendreminder")]
        [HttpPost]
        public IHttpActionResult SendReminder([FromBody]InputData data)
        {
            try
            {
                _service.SendReminder(data.Id);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("setpnr")]
        [HttpPost]
        public IHttpActionResult SetPnr([FromBody]PnrData data)
        {
            try
            {
                _service.SetPnr(data.Id, data.Pnr);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}