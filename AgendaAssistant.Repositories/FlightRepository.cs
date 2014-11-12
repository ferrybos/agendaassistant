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
        Flight Get(string departureStation, string arrivalStation, DateTime departureDate, string carrierCode, short flightNumber, short paxCount);
    }

    public class FlightRepository : IFlightRepository
    {
        private PaxPriceType[] PaxPriceTypes
        {
            get
            {
                return new[] { new PaxPriceType() { PaxType = "ADT", PaxDiscountCode = "DAGT" } };
            }
        }

        public List<Flight> Search(string departureStation, string arrivalStation, DateTime beginDate, DateTime endDate, short paxCount,
            BitArray daysOfWeek, short? maxPrice)
        {
            var esbClient = new EsbApi34();

            esbClient.LogOn();
            try
            {
                var availabilityRequest = esbClient.BookingManager.CreateAvailabilityRequest(departureStation.Trim(),
                                                                                             arrivalStation.Trim(),
                                                                                             beginDate, endDate,
                                                                                             carrierCode: null,
                                                                                             paxCount: paxCount,
                                                                                             paxPriceTypes:
                                                                                                 PaxPriceTypes);

                var availabilityResponse =
                    esbClient.BookingManager.GetAvailability(new TripAvailabilityRequest()
                        {
                            AvailabilityRequests = new[] { availabilityRequest }
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
                        var price = FarePrice(availableFare);

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

        public Flight Get(string departureStation, string arrivalStation, DateTime departureDate, string carrierCode,
                          short flightNumber, short paxCount)
        {
            var esbClient = new EsbApi34();

            esbClient.LogOn();
            try
            {
                // add 
                var availabilityRequest = esbClient.BookingManager.CreateAvailabilityRequest(departureStation.Trim(),
                                                                                             arrivalStation.Trim(),
                                                                                             departureDate.Date,
                                                                                             departureDate.Date,
                                                                                             carrierCode,
                                                                                             flightNumber.ToString()
                                                                                                         .PadLeft(4, ' '),
                                                                                             paxCount,
                                                                                             paxPriceTypes:
                                                                                                 PaxPriceTypes
                    );

                var availabilityResponse =
                    esbClient.BookingManager.GetAvailability(new TripAvailabilityRequest()
                    {
                        AvailabilityRequests = new[] { availabilityRequest }
                    });

                var journey = availabilityResponse.Journeys().Single();
                var segment = journey.Segments[0];

                var availableFare = (AvailableFare)journey.Fares().FirstOrDefault();

                if (availableFare != null)
                {
                    var price = FarePrice(availableFare);

                    return new Flight()
                        {
                            DepartureStation = segment.DepartureStation,
                            ArrivalStation = segment.ArrivalStation,
                            DepartureDate = segment.STD.Date,
                            CarrierCode = segment.FlightDesignator.CarrierCode,
                            FlightNumber = short.Parse(segment.FlightDesignator.FlightNumber),
                            STD = segment.STD,
                            STA = segment.STA,
                            Price = price
                        };
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                esbClient.LogOut();
            }
        }

        private static decimal FarePrice(AvailableFare availableFare)
        {
            var price = availableFare.SumOfAmount(ChargeType.FarePrice) +
                        availableFare.SumOfAmount(ChargeType.Tax) -
                        availableFare.SumOfAmount(ChargeType.Discount);
            return price;
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
