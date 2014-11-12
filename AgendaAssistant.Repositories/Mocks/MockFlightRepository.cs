using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Entities;

namespace AgendaAssistant.Repositories.Mocks
{
    public class MockFlightRepository : IFlightRepository
    {
        public List<Flight> Search(string departureStation, string arrivalStation, DateTime beginDate, DateTime endDate,
                                   short paxCount, BitArray daysOfWeek, short? maxPrice)
        {
            var result = new List<Flight>();

            var totalDays = (endDate - beginDate).TotalDays;
            for (int dayIndex = 0; dayIndex <= totalDays; dayIndex++)
            {
                var day = beginDate.AddDays(dayIndex);
                result.Add(new Flight()
                    {
                        DepartureStation = departureStation,
                        ArrivalStation = arrivalStation,
                        DepartureDate = day,
                        STD = day.AddHours(8),
                        STA = day.AddHours(10),
                        CarrierCode = "HV",
                        FlightNumber = 123,
                        Price = 100
                    });
            }

            return result;
        }

        public Flight Get(string departureStation, string arrivalStation, DateTime departureDate, string carrierCode,
                          short flightNumber, short paxCount)
        {
            throw new NotImplementedException();
        }
    }
}
