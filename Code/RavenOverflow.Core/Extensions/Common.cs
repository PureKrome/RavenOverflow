using System;

namespace RavenOverflow.Core.Extensions
{
    public static class Common
    {
        public static DateTime ToUtcToday(this DateTime dateTime)
        {
            DateTime utcNow = DateTime.UtcNow;
            return new DateTime(utcNow.Year, utcNow.Month, utcNow.Day);
        }
    }
}