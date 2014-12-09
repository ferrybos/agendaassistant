using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vluchtprikker.Entities;

namespace Vluchtprikker.Extensions
{
    public static class PersonExtender
    {
        public static bool Matches(this Person person, string name, string email)
        {
            return person.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) &&
                   person.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
