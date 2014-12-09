using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vluchtprikker.Entities;

namespace Vluchtprikker.Repositories.Mocks
{
    public class MockFlightRepository : IFlightRepository
    {
        public FlightSearchResponse Search(string departureStation, string arrivalStation, DateTime beginDate, DateTime endDate,
                                   short paxCount, BitArray daysOfWeek, short? maxPrice)
        {
            var response = FlightSearchResponseFactory.New();

            var totalDays = (endDate - beginDate).TotalDays;
            for (int dayIndex = 0; dayIndex <= totalDays; dayIndex++)
            {
                var day = beginDate.AddDays(dayIndex);
                response.OutboundFlights.Add(new Flight()
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

                response.InboundFlights.Add(new Flight()
                {
                    DepartureStation = arrivalStation,
                    ArrivalStation = departureStation,
                    DepartureDate = day,
                    STD = day.AddHours(8),
                    STA = day.AddHours(10),
                    CarrierCode = "HV",
                    FlightNumber = 123,
                    Price = 100
                });
            }

            return response;
        }
    }
}
