using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaAssistant.Web.models
{
    public class Participant
    {
        public Person Person;

        public string FirstNameInPassport;
        public string LastNameInPassport;
        public DateTime? DateOfBirth;
        public short? Gender;
        public string Bagage;
        public string Status;
    }
}
