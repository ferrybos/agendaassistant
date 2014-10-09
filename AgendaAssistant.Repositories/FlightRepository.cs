using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Entities;
using ESB_API_34;
using ESB_API_34.API34;
using ESB_API_34.API34.BookingManager;

namespace AgendaAssistant.Repositories
{
    public interface IFlightRepository
    {
        List<Flight> Search(string departureStation, string arrivalStation, DateTime beginDate, DateTime endDate, short paxCount);
    }

    public class FlightRepository : IFlightRepository
    {
        public List<Flight> Search(string departureStation, string arrivalStation, DateTime beginDate, DateTime endDate, short paxCount)
        {
            var esbClient = new EsbApi34();
            
            esbClient.LogOn();
            try
            {
                var availabilityRequest = esbClient.BookingManager.CreateAvailabilityRequest(departureStation, arrivalStation, beginDate, endDate,
                                                               carrierCode: null, paxCount: paxCount);
                var availabilityResponse =
                    esbClient.BookingManager.GetAvailability(new TripAvailabilityRequest()
                        {
                            AvailabilityRequests = new [] {availabilityRequest}
                        });

                var journeys = availabilityResponse.Journeys();

                var result = new List<Flight>();
                foreach (var journey in journeys)
                {
                    var availableFare = (AvailableFare)journey.Fares().FirstOrDefault();

                    if (availableFare != null)
                    {
                        var segment = journey.Segments[0];
                        result.Add(new Flight()
                        {
                            DepartureStation = segment.DepartureStation,
                            ArrivalStation = segment.ArrivalStation,
                            CarrierCode = segment.FlightDesignator.CarrierCode,
                            FlightNumber = short.Parse(segment.FlightDesignator.FlightNumber),
                            STD = segment.STD,
                            STA = segment.STA,
                            Price = availableFare.SumOfAmount(ChargeType.FarePrice) + availableFare.SumOfAmount(ChargeType.Tax) - availableFare.SumOfAmount(ChargeType.Discount)
                        });
                    }
                }

                return result;
            }
            finally
            {
                esbClient.LogOut();
            }
        }
    }
}
