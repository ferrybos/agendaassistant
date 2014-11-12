using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AgendaAssistant.Entities;
using AgendaAssistant.Services;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Web.api
{
    public class NewEventData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class InputData
    {
        public string Id { get; set; }
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
        private readonly IAvailabilityService _availabilityService;
        private readonly IFlightService _flightService;

        public EventController(IEventService eventService, IAvailabilityService availabilityService, IFlightService flightService)
        {
            _service = eventService;
            _availabilityService = availabilityService;
            _flightService = flightService;
        }

        // GET api/<controller>/5
        [Route("{id}")]
        [HttpGet]
        public Event Get(string id)
        {
            var evn = _service.Get(id);

            var eventAvailabilities = _availabilityService.GetByEvent(id);
            evn.AddAvailabilities(eventAvailabilities);

            // used to show participants that have not reacted yet (clicked the link in the confirmation email)
            foreach (var participant in evn.Participants)
            {
                participant.HasConfirmed = eventAvailabilities.Any(a => a.ParticipantId == participant.Id);
            }

            var organizerParticipant = evn.Participants.SingleOrDefault(p => p.Person.Id.Equals(evn.Organizer.Id));

            if (organizerParticipant != null)
                evn.OrganizerParticipantCode = organizerParticipant.Id;

            return evn;
        }

        // POST api/<controller>
        [Route("")]
        [HttpPost]
        public IHttpActionResult Post([FromBody]Event data)
        {
            //System.Threading.Thread.Sleep(2000);

            // create new event
            try
            {
                var newEvent = _service.Create(data.Title, data.Description, data.Organizer.Name, data.Organizer.Email);

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
    }
}