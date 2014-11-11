using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Repositories;
using AgendaAssistant.Entities;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Services
{
    public interface IStationService
    {
        List<Station> GetStations();
        List<Route> GetRoutes();
    }

    public class StationService : IStationService
    {
        private readonly IDbContext _dbContext;
        private readonly StationRepository _repository;

        public StationService(IDbContext dbContext)
        {
            _dbContext = dbContext;
            _repository = new StationRepository(_dbContext);
        }

        public List<Station> GetStations()
        {
            var dbStations = _repository.GetStations();

            var entities = new List<Station>();
            dbStations.ForEach(s => entities.Add(EntityMapper.Map(s)));

            return entities;
        }

        public List<Route> GetRoutes()
        {
            var dbRoutes = _repository.GetRoutes();

            var entities = new List<Route>();
            dbRoutes.ForEach(r => entities.Add(EntityMapper.Map(r)));

            return entities;
        }
    }
}
