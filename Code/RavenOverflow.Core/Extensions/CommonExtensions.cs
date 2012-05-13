using System;

namespace RavenOverflow.Core.Extensions
{
    public static class CommonExtensions
    {
        public static DateTime ToUtcToday(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }
    }
}