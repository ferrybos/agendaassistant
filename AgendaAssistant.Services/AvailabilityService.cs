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

        public List<Availability> Get(long flightSearchId, long personId)
        {
            return _repository.Get(flightSearchId, personId);
        }
    }

    public interface IAvailabilityService
    {
        List<Availability> Get(long flightSearchId, long personId);
    }
}
