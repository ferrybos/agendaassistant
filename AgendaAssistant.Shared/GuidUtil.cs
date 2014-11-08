using System;

namespace AgendaAssistant.Shared
{
    public static class GuidUtil
    {
        public static string ToString(Guid value)
        {
            return value.ToString().ToLower().Replace("-", "");
        }

        public static Guid ToGuid(string value)
        {
            if (value.Length != 32)
                throw new FormattedException("Code '{0}' is invalid", value);

            var guidString = value.Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-");

            return new Guid(guidString);
        }
    }
}
