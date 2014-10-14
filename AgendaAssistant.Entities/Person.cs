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
            return new Person() { Id = 0 };
        }
    }

    public class Person
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
