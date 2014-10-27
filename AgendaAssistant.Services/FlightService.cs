using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Repositories;
using AgendaAssistant.Entities;

namespace AgendaAssistant.Services
{
    public interface IFlightService
    {
        List<Flight> Search(string departureStation, string arrivalStation, DateTime beginDate, DateTime endDate, short paxCount, BitArray daysOfWeek, short? maxPrice);
    }

    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _repository;

        public FlightService(IFlightRepository repository)
        {
            _repository = repository;
        }

        public List<Flight> Search(string departureStation, string arrivalStation, DateTime beginDate, DateTime endDate, short paxCount, BitArray daysOfWeek, short? maxPrice)
        {
            return _repository.Search(departureStation, arrivalStation, beginDate, endDate, paxCount, daysOfWeek, maxPrice);
        }
    }
}
