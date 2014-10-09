using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Entities;

namespace AgendaAssistant.Repositories
{
    public interface IFlightRepository
    {
        List<Flight> Search(string departureStation, string arrivalStation, DateTime beginDate, DateTime endDate);
    }

    public class FlightRepository : IFlightRepository
    {
        public List<Flight> Search(string departureStation, string arrivalStation, DateTime beginDate, DateTime endDate)
        {
            // todo: call ESB
            var result = new List<Flight>();

            var totalDays = (endDate - beginDate).TotalDays + 1; // including endDate
            for (int dayIndex = 0; dayIndex < totalDays; dayIndex++)
            {
                var date = beginDate.AddDays(dayIndex);
                result.Add(new Flight() { DepartureStation = departureStation, ArrivalStation = arrivalStation, CarrierCode = "HV", FlightNumber = 5131, STD = date.AddHours(1), STA = date.AddHours(2), Price = 100 });
                result.Add(new Flight() { DepartureStation = departureStation, ArrivalStation = arrivalStation, CarrierCode = "HV", FlightNumber = 5135, STD = date.AddHours(5), STA = date.AddHours(6), Price = 120 });
            }

            return result; 
        }
    }
}
