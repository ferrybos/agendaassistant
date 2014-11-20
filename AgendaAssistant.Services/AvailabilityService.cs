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
        //List<Availability> GetByEvent(string eventId);
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

        //public List<Availability> GetByEvent(string eventId)
        //{
        //    var dbEvent = new EventRepository(_dbContext).Single(GuidUtil.ToGuid(eventId));

        //    var result = new List<Availability>();

        //    if (dbEvent.OutboundFlightSearchID.HasValue)
        //        _repository.SelectAll(dbEvent.OutboundFlightSearchID.Value).ForEach(a => result.Add(EntityMapper.Map(a)));

        //    if (dbEvent.InboundFlightSearchID.HasValue)
        //        _repository.SelectAll(dbEvent.InboundFlightSearchID.Value).ForEach(a => result.Add(EntityMapper.Map(a)));

        //    return result;
        //}

        public Event Get(string participantId)
        {
            var id = GuidUtil.ToGuid(participantId);

            var dbParticipant = new ParticipantRepository(_dbContext).Single(id);
            var dbEvent = new EventRepository(_dbContext).Single(dbParticipant.EventID);

            var eventIsActive = dbEvent.PNR == null;

            // als vluchten zijn geprikt, hoeven overige vluchten niet meer geladen te worden

            var evn = EntityMapper.Map(dbEvent, includeParticipants: false, includeFlights: eventIsActive,
                                       includeAvailability: eventIsActive);

            // don't add other participant data
            evn.Participants.Add(EntityMapper.Map(dbParticipant)); 

            if (eventIsActive)
            {
                bool participantHasAvailability = dbParticipant.Availabilities.Any();

                if (!participantHasAvailability)
                {
                    // first visit, create if not exist yet (first time participant visits the event)
                    foreach (var dbFlight in dbEvent.OutboundFlightSearch.Flights)
                    {
                        _repository.Create(dbParticipant, dbFlight);
                    }

                    foreach (var dbFlight in dbEvent.InboundFlightSearch.Flights)
                    {
                        _repository.Create(dbParticipant, dbFlight);
                    }

                    _dbContext.Current.SaveChanges();
                }

                // attach availability to flights
                if (!dbParticipant.AvailabilityConfirmed) // else the availability is already included
                {
                    var flightsDictionary = new Dictionary<long, Flight>();
                    evn.OutboundFlightSearch.Flights.ForEach(f => flightsDictionary.Add(f.Id, f));
                    evn.InboundFlightSearch.Flights.ForEach(f => flightsDictionary.Add(f.Id, f));

                    foreach (var dbAvailability in dbParticipant.Availabilities)
                    {
                        flightsDictionary[dbAvailability.FlightID].Availabilities.Add(EntityMapper.Map(dbAvailability,
                                                                                                       dbParticipant
                                                                                                           .Name));
                    }
                }

                MoveParticipantAvailability(participantId, evn.OutboundFlightSearch);
                MoveParticipantAvailability(participantId, evn.InboundFlightSearch);
            }

            // remove personal data

            return evn;
        }

        private void MoveParticipantAvailability(string participantId, FlightSearch flightSearch)
        {
            foreach (var flight in flightSearch.Flights)
            {
                flight.ParticipantAvailability =
                    flight.Availabilities.Single(a => a.ParticipantId.Equals(participantId));

                flight.Availabilities.Remove(flight.ParticipantAvailability);

                // reduce JSON response
                flight.Availabilities.ForEach(a => a.ParticipantId = "");
            }
        }

        public void Update(Availability availability)
        {
            var dbParticipant = new ParticipantRepository(_dbContext).Single(GuidUtil.ToGuid(availability.ParticipantId));

            _repository.Update(dbParticipant.ID, availability.FlightId, availability.Value, availability.CommentText);

            
        }

        public void Confirm(string participantId)
        {
            var dbParticipant = new ParticipantRepository(_dbContext).Single(GuidUtil.ToGuid(participantId));
            var dbEvent = new EventRepository(_dbContext).Single(dbParticipant.EventID);

            // dont send to myself if the organizer is a participant also
            if (!dbParticipant.Email.Equals(dbEvent.OrganizerEmail))
                _mailService.SendAvailabilityUpdate(dbEvent, dbParticipant);

            dbParticipant.AvailabilityConfirmed = true;
            _dbContext.Current.SaveChanges();
        }
    }
}
