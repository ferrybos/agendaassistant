using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaAssistant.Entities
{
    public enum Gender
    {
        Male,
        Female
    }

    public class Person
    {
        public string Id { get; set; }

        public string Email { get; set; }
        public string Name { get; set; }

        public string FirstNameInPassport { get; set; }
        public string LastNameInPassport { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

    }
}
