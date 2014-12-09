using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vluchtprikker.Shared;

namespace Vluchtprikker.Repositories
{
    public static class CodeString
    {
        public static string GuidAsCodeString(Guid value)
        {
            return value.ToString().ToLower().Replace("-", "");
        }

        public static Guid CodeStringToGuid(string value)
        {
            if (value.Length != 32)
                throw new FormattedException("Code '{0}' is invalid", value);

            var guidString = value.Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-");

            return new Guid(guidString);
        }
    }
}
