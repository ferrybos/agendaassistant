using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Vluchtprikker.Entities
{
    public class Flight
    {
        public long Id { get; set; }

        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime STD { get; set; }
        public DateTime STA { get; set; }
        public string CarrierCode { get; set; }
        public short FlightNumber { get; set; }
        public decimal Price { get; set; }
        public bool Enabled { get; set; }
        
        [JsonProperty(PropertyName = "av")]
        public List<Availability> Availabilities = new List<Availability>();

        [JsonProperty(PropertyName = "paav")]
        public Availability ParticipantAvailability { get; set; }

        public decimal Green
        {
            get
            {
                decimal count = Availabilities.Count;

                if (count == 0)
                    return 0;

                return (Availabilities.Count(a => a.Value == 100)/count)*100;
            }
        }

        public decimal Orange
        {
            get
            {
                decimal count = Availabilities.Count;

                if (count == 0)
                    return 0;

                return (Availabilities.Count(a => a.Value == 50) / count) * 100;
            }
        }

        public decimal Red
        {
            get
            {
                decimal count = Availabilities.Count;

                if (count == 0)
                    return 0;

                return (Availabilities.Count(a => a.Value == 0) / count) * 100;
            }
        }
    }
}
