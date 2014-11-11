using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaAssistant.Entities
{
    public class Route
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
    }

    public class Station
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
