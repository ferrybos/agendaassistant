using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vluchtprikker.Shared;
using Newtonsoft.Json;

namespace Vluchtprikker.Entities
{
    public class Participant
    {
        public string Id { get; set; }

        public string EventId { get; set; }
        
        public Person Person { get; set; }
        
        public string Bagage { get; set; }

        [JsonProperty(PropertyName = "AvConfirmed")]
        public bool AvailabilityConfirmed { get; set; }

        [JsonProperty(PropertyName = "BdConfirmed")]
        public bool BookingDetailsConfirmed { get; set; }


    }
}
