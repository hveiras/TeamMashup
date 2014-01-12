using System;

namespace TeamMashup.Server
{
    public static class DateTimeExtensions
    {
        public static DateTime Epoch
        {
            get { return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc); }
        }

        public static long GetMillisecondsFromEpoch(this DateTime dateTime)
        {
            var delta = dateTime - Epoch;

            return (long)Math.Round(delta.TotalMilliseconds, 0);
        }
    }
}