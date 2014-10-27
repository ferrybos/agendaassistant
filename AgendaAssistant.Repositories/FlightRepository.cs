using System;
using System.Collections;
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
        List<Flight> Search(string departureStation, string arrivalStation, DateTime beginDate, DateTime endDate, short paxCount, BitArray daysOfWeek, short? maxPrice);
    }

    public class FlightRepository : IFlightRepository
    {
        public List<Flight> Search(string departureStation, string arrivalStation, DateTime beginDate, DateTime endDate, short paxCount,
            BitArray daysOfWeek, short? maxPrice)
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
                        
                        var departureDate = segment.STD.Date;
                        var price = availableFare.SumOfAmount(ChargeType.FarePrice) +
                                    availableFare.SumOfAmount(ChargeType.Tax) -
                                    availableFare.SumOfAmount(ChargeType.Discount);

                        if (maxPrice.HasValue && price > maxPrice.Value)
                            continue;

                        if (ContainsDayOfWeek(daysOfWeek, departureDate.DayOfWeek))
                        {
                            result.Add(new Flight()
                            {
                                DepartureStation = segment.DepartureStation,
                                ArrivalStation = segment.ArrivalStation,
                                DepartureDate = departureDate,
                                CarrierCode = segment.FlightDesignator.CarrierCode,
                                FlightNumber = short.Parse(segment.FlightDesignator.FlightNumber),
                                STD = segment.STD,
                                STA = segment.STA,
                                Price = price
                            });
                        }
                    }
                }

                return result;
            }
            finally
            {
                esbClient.LogOut();
            }
        }

        private bool ContainsDayOfWeek(BitArray daysOfWeek, DayOfWeek dayOfWeek)
        {
            return (daysOfWeek[0] && dayOfWeek == DayOfWeek.Monday) || (daysOfWeek[1] && dayOfWeek == DayOfWeek.Tuesday) ||
                   (daysOfWeek[2] && dayOfWeek == DayOfWeek.Wednesday) ||
                   (daysOfWeek[3] && dayOfWeek == DayOfWeek.Thursday) ||
                   (daysOfWeek[4] && dayOfWeek == DayOfWeek.Friday) ||
                   (daysOfWeek[5] && dayOfWeek == DayOfWeek.Saturday) ||
                   (daysOfWeek[6] && dayOfWeek == DayOfWeek.Sunday);

        }
    }
}
