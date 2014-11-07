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
    /// <summary>
    /// Contains all business logic regarding events
    /// </summary>
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IAvailabilityRepository _repository;

        public AvailabilityService(IAvailabilityRepository repository)
        {
            _repository = repository;
        }

        public short CalculateAvailabilityPercentage(Flight flight)
        {
            return flight.Availabilities.Count == 0 ? (short) 0 : (short) (flight.Availabilities.Average(a => a.Value));
        }

        public List<Availability> Get(long flightSearchId)
        {
            return _repository.Get(flightSearchId);            
        }

        public List<Availability> Get(long flightSearchId, long personId)
        {
            return _repository.Get(flightSearchId, personId);
        }

        public void Update(Availability availability)
        {
            _repository.Update(availability);

            // todo: insert mail record if no unsent record exists already

        }
    }

    public interface IAvailabilityService
    {
        List<Availability> Get(long flightSearchId, long personId);
        List<Availability> Get(long flightSearchId);
        short CalculateAvailabilityPercentage(Flight flight);
        void Update(Availability availability);
    }
}
