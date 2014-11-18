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
        Event Get(string participantId);
        List<Availability> GetByEvent(string eventId);
        short CalculateAvailabilityPercentage(Flight flight);
        void Update(Availability availability);
        void Confirm(string participantId);
    }

    /// <summary>
    /// Contains all business logic regarding events
    /// </summary>
    public class AvailabilityService : IAvailabilityService
    {
        private readonly AvailabilityRepository _repository;
        private readonly IDbContext _dbContext;
        private readonly IMailService _mailService;

        public AvailabilityService(IDbContext dbContext, IMailService mailService)
        {
            _dbContext = dbContext;
            _repository = new AvailabilityRepository(_dbContext);
            _mailService = mailService;
        }

        public short CalculateAvailabilityPercentage(Flight flight)
        {
            return flight.Availabilities.Count == 0 ? (short) 0 : (short) (flight.Availabilities.Average(a => a.Value));
        }

        public List<Availability> GetByEvent(string eventId)
        {
            var dbEvent = new EventRepository(_dbContext).Single(GuidUtil.ToGuid(eventId));

            var result = new List<Availability>();

            if (dbEvent.OutboundFlightSearchID.HasValue)
                _repository.SelectAll(dbEvent.OutboundFlightSearchID.Value).ForEach(a => result.Add(EntityMapper.Map(a)));

            if (dbEvent.InboundFlightSearchID.HasValue)
                _repository.SelectAll(dbEvent.InboundFlightSearchID.Value).ForEach(a => result.Add(EntityMapper.Map(a)));

            return result;
        }

        public Event Get(string participantId)
        {
            Guid id = GuidUtil.ToGuid(participantId);

            var dbParticipant = new ParticipantRepository(_dbContext).Single(id);
            var dbEvent = new EventRepository(_dbContext).Single(dbParticipant.EventID);

            var evn = EntityMapper.Map(dbEvent, includeParticipants: false);
            evn.Participants.Add(EntityMapper.Map(dbParticipant));

            var dbAvailabilities = _repository.SelectAll(id);

            if (dbAvailabilities.Count == 0)
            {
                // create if not exist yet (first time participant visits the event)
                dbAvailabilities.AddRange(evn.OutboundFlightSearch.Flights.Select(flight => _repository.Create(id, flight.Id)));
                dbAvailabilities.AddRange(evn.InboundFlightSearch.Flights.Select(flight => _repository.Create(id, flight.Id)));
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
            var dbParticipant = new ParticipantRepository(_dbContext).Single(GuidUtil.ToGuid(availability.ParticipantId));

            _repository.Update(dbParticipant.ID, availability.FlightId, availability.Value, availability.CommentText);

            dbParticipant.AvailabilityConfirmed = true;
            _dbContext.Current.SaveChanges();
        }

        public void Confirm(string participantId)
        {
            var dbParticipant = new ParticipantRepository(_dbContext).Single(GuidUtil.ToGuid(participantId));
            var dbEvent = new EventRepository(_dbContext).Single(dbParticipant.EventID);

            _mailService.SendAvailabilityUpdate(dbEvent, dbParticipant);
        }
    }
}
