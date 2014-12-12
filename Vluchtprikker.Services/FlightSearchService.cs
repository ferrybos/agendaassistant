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
    public class FlightSearchService
    {
        private readonly IDbContext _dbContext;

        public FlightSearchService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddStationNames(Event evn)
        {
            var stationRepository = new StationRepository(_dbContext);

            evn.OutboundFlightSearch.DepartureStationName = stationRepository.Single(evn.OutboundFlightSearch.DepartureStation).Name;
            evn.OutboundFlightSearch.ArrivalStationName = stationRepository.Single(evn.OutboundFlightSearch.ArrivalStation).Name;
            evn.InboundFlightSearch.DepartureStationName = stationRepository.Single(evn.InboundFlightSearch.DepartureStation).Name;
            evn.InboundFlightSearch.ArrivalStationName = stationRepository.Single(evn.InboundFlightSearch.ArrivalStation).Name;
        }
    }
}
