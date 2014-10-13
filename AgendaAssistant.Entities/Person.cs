using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaAssistant.Entities
{
    public static class PersonFactory
    {
        public static Person NewPerson()
        {
            return new Person() { PersonId = 0 };
        }
    }

    public class Person
    {
        public long PersonId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
