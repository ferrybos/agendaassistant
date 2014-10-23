using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaAssistant.Entities
{
    public class Participant
    {
        public long PersonId { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        public bool HasConfirmed { get; set; }
    }
}
