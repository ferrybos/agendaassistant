using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Entities;
using AgendaAssistant.Repositories;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Services
{
    public interface IAvailabilityService
    {
        Event Get(Guid participantId);
        List<Availability> GetByEvent(Guid eventId);
        short CalculateAvailabilityPercentage(Flight flight);
        void Update(Availability availability);
    }

    /// <summary>
    /// Contains all business logic regarding events
    /// </summary>
    public class AvailabilityService : IAvailabilityService
    {
        private readonly AvailabilityRepository _repository;
        private readonly IDbContext _dbContext;

        public AvailabilityService(IDbContext dbContext)
        {
            _dbContext = dbContext;
            _repository = new AvailabilityRepository(_dbContext);
        }

        public short CalculateAvailabilityPercentage(Flight flight)
        {
            return flight.Availabilities.Count == 0 ? (short) 0 : (short) (flight.Availabilities.Average(a => a.Value));
        }

        public List<Availability> GetByEvent(Guid eventId)
        {
            var dbEvent = new EventRepository(_dbContext).Single(eventId);

            var result = new List<Availability>();

            if (dbEvent.OutboundFlightSearchID.HasValue)
                _repository.SelectAll(dbEvent.OutboundFlightSearchID.Value).ForEach(a => result.Add(EntityMapper.Map(a)));

            if (dbEvent.InboundFlightSearchID.HasValue)
                _repository.SelectAll(dbEvent.InboundFlightSearchID.Value).ForEach(a => result.Add(EntityMapper.Map(a)));

            return result;
        }

        public Event Get(Guid participantId)
        {
            var dbParticipant = new ParticipantRepository(_dbContext).Single(participantId);
            var dbEvent = new EventRepository(_dbContext).Single(dbParticipant.EventID);

            var evn = EntityMapper.Map(dbEvent, includeParticipants: false);
            evn.Participants.Add(EntityMapper.Map(dbParticipant));

            var dbAvailabilities = _repository.SelectAll(participantId);

            if (dbAvailabilities.Count == 0)
            {
                // create if not exist yet (first time participant visits the event)
                dbAvailabilities.AddRange(evn.OutboundFlightSearch.Flights.Select(flight => _repository.Create(participantId, flight.Id)));
                dbAvailabilities.AddRange(evn.InboundFlightSearch.Flights.Select(flight => _repository.Create(participantId, flight.Id)));
            }

            var availabilities = new List<Availability>();
            dbAvailabilities.ForEach(a => availabilities.Add(EntityMapper.Map(a)));

            // map availabilities to flightsearch
            evn.OutboundFlightSearch.AddAvailabilities(availabilities);
            evn.InboundFlightSearch.AddAvailabilities(availabilities);

            return evn;
        }

        public void Update(Availability availability)
        {
            var dbParticipant = new ParticipantRepository(_dbContext).Single(availability.ParticipantId);

            _repository.Update(availability.ParticipantId, availability.FlightId, availability.Value,
                               availability.CommentText);

            // trigger new email to be sent by web job
            dbParticipant.AvailabilityUpdateSent = false;
            _dbContext.Current.SaveChanges();
        }
    }
}
