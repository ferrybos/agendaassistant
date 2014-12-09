using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vluchtprikker.Entities;
using Vluchtprikker.Repositories;
using Vluchtprikker.Shared;

namespace Vluchtprikker.Services
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

                CopyParticipantAvailability(participantId, evn.OutboundFlightSearch);
                CopyParticipantAvailability(participantId, evn.InboundFlightSearch);
            }

            // remove personal data

            return evn;
        }

        private void CopyParticipantAvailability(string participantId, FlightSearch flightSearch)
        {
            foreach (var flight in flightSearch.Flights)
            {
                flight.ParticipantAvailability =
                    flight.Availabilities.Single(a => a.ParticipantId.Equals(participantId));

                // reduce JSON response
                //flight.Availabilities.ForEach(a => a.ParticipantId = "");
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

            var isNotComplete = dbParticipant.Availabilities.Any(a => !a.Value.HasValue);
            if (isNotComplete)
            {
                throw new ApplicationException("U heeft nog niet alle beschikbaarheid ingevuld.");
            }

            var dbEvent = new EventRepository(_dbContext).Single(dbParticipant.EventID);

            // dont send to myself if the organizer is a participant also
            if (!dbParticipant.Email.Equals(dbEvent.OrganizerEmail))
                _mailService.SendAvailabilityUpdate(dbEvent, dbParticipant);

            dbParticipant.AvailabilityConfirmed = true;
            _dbContext.Current.SaveChanges();
        }
    }
}
