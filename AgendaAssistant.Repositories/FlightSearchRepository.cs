using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.DB.Repositories;
using AgendaAssistant.Entities;
using AgendaAssistant.Extensions;
using AgendaAssistant.Shared;
using FlightSearch = AgendaAssistant.DB.FlightSearch;

namespace AgendaAssistant.Repositories
{
    /// <summary>
    /// Contains all logic to interface with data(base)
    /// </summary>
    public class FlightSearchRepository : DbRepository
    {
        public FlightSearchRepository(IDbContext dbContext)
            : base(dbContext)
        {
        }

        public FlightSearch Add(string departureStation, string arrivalStation, DateTime beginDate, DateTime endDate, short daysOfWeek, short? maxPrice)
        {
            var dbFlightSearch = DbContext.FlightSearches.Create();
            DbContext.FlightSearches.Add(dbFlightSearch);

            dbFlightSearch.DepartureStation = departureStation;
            dbFlightSearch.ArrivalStation = arrivalStation;
            dbFlightSearch.StartDate = beginDate;
            dbFlightSearch.EndDate = endDate;
            dbFlightSearch.DaysOfWeek = daysOfWeek;
            dbFlightSearch.MaxPrice = maxPrice;

            DbContext.SaveChanges();

            return dbFlightSearch;
        }

        public void AddFlight(FlightSearch dbFlightSearch,
            string carrierCode, int flightNumber, DateTime departureDate, DateTime sta, DateTime std, int price)
        {
            var dbFlight = DbContext.Flights.Create();
            dbFlightSearch.Flights.Add(dbFlight);

            dbFlight.CarrierCode = carrierCode;
            dbFlight.DepartureDate = departureDate;
            dbFlight.FlightNumber = flightNumber;
            dbFlight.Price = price;
            dbFlight.STA = sta;
            dbFlight.STD = std;
            dbFlight.Enabled = true;

            DbContext.SaveChanges();
        }

        public FlightSearch Single(long id)
        {
            var dbFlightSearch = DbContext.FlightSearches.SingleOrDefault(e => e.ID.Equals(id));

            if (dbFlightSearch == null)
            {
                throw new ApplicationException(string.Format("FlightSearch not found with id {0}", id));
            }

            return dbFlightSearch;
        }
    }
}
