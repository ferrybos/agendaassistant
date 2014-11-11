using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.DB;
using AgendaAssistant.DB.Repositories;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Repositories
{
    public class StationRepository : DbRepository
    {
        public StationRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        public List<Station> GetStations()
        {
            return DbContext.Stations.ToList();
        }

        public List<Route> GetRoutes()
        {
            return DbContext.Routes.ToList();
        }
    }
}
