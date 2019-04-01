using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyFavoriteThings
{
    public static class ToTimeStamp
    {
        public static double ToTimestamp(this DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}