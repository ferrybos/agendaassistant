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
    public class ConfirmData
    {
        public string Code { get; set; }
    }

    [RoutePrefix("api/event")]
    public class EventController : ApiController
    {
        private readonly IEventService _service;
        private readonly IAvailabilityService _availabilityService;

        public EventController(IEventService eventService, IAvailabilityService availabilityService)
        {
            _service = eventService;
            _availabilityService = availabilityService;
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
            // todo: remove duplicate code from AvailabilityController
            var evn = _service.Get(id);

            var outboundAvailabilities = _availabilityService.Get(evn.OutboundFlightSearch.Id);
            foreach (var flight in evn.OutboundFlightSearch.Flights)
            {
                flight.Availabilities = new List<Availability>();
                flight.Availabilities.AddRange(outboundAvailabilities.Where(a => a.FlightId == flight.Id));
                flight.AvailabilityPercentage = _availabilityService.CalculateAvailabilityPercentage(flight);
            }

            var inboundAvailabilities = _availabilityService.Get(evn.InboundFlightSearch.Id);
            foreach (var flight in evn.InboundFlightSearch.Flights)
            {
                flight.Availabilities = new List<Availability>();
                flight.Availabilities.AddRange(inboundAvailabilities.Where(a => a.FlightId == flight.Id));
                flight.AvailabilityPercentage = _availabilityService.CalculateAvailabilityPercentage(flight);
            }

            // used to show participants that have not reacted yet (clicked the link in the confirmation email)
            foreach (var participant in evn.Participants)
            {
                participant.HasConfirmed = outboundAvailabilities.Any(a => a.PersonId == participant.PersonId) ||
                                           inboundAvailabilities.Any(a => a.PersonId == participant.PersonId);
            }

            return evn;
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
        public IHttpActionResult Confirm([FromBody]ConfirmData data)
        {
            try
            {
                _service.Confirm(data.Code);

                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("selectflight")]
        [HttpPost]
        public IHttpActionResult SelectFlight(long flightSearchId, long flightId)
        {
            try
            {
                _service.SelectFlight(flightSearchId, flightId);
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