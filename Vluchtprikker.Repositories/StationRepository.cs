using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vluchtprikker.DB;
using Vluchtprikker.DB.Repositories;
using Vluchtprikker.Shared;

namespace Vluchtprikker.Repositories
{
    public class StationRepository : DbRepository
    {
        public StationRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        public Station Single(string iataCode)
        {
            return DbContext.Stations.Single(s => s.Code.Equals(iataCode));
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
