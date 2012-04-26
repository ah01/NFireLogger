using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFireLogger.Utils
{
    internal static class DateTimeExtensions
    {
        /// <summary>
        /// Return UNIX style timestemp of date
        /// </summary>
        /// <param name="time"></param>
        /// <returns>Unix timestemp</returns>
        public static long ToTimestamp(this DateTime time)
        {
            var origin = new DateTime(1970, 1, 1);

            return (long) Math.Max((time - origin).TotalSeconds, 0);
        }
    }
}
