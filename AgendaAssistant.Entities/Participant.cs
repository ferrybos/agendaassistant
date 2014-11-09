using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgendaAssistant.Shared;

namespace AgendaAssistant.Entities
{
    public class Participant
    {
        public string Id { get; set; }

        public string EventId { get; set; }
        
        public Person Person { get; set; }

        public bool HasConfirmed { get; set; }

        public string Bagage { get; set; }

    }
}
