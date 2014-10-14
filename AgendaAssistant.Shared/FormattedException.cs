using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgendaAssistant.Shared
{
    public class FormattedException: Exception
    {
        public FormattedException(string errorMsg, params object[] args)
            : base(string.Format(errorMsg, args))
        { }

        public FormattedException(string errorMsg, Exception innerException, params object[] args)
            : base(string.Format(errorMsg, args), innerException)
        { }
    }
}
