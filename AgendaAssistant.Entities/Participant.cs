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
        public Guid Id { get; set; }

        public string Code
        {
            get { return GuidUtil.ToString(Id); }
        }

        public Guid EventId { get; set; }
        
        public Person Person { get; set; }

        public bool HasConfirmed { get; set; }

        public string Bagage { get; set; }

    }
}
