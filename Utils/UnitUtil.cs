using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Utils
{
    public enum TimeUnit
    {
        Millisecond = 1,
        Second = (Millisecond * 1000),
        Minute = (Second * 60),
        Hour = (Minute * 60),
        Day = (Hour * 24),
        Week = (Day * 7)
    }
    public static class UnitUtil
    {
        private const decimal OuncesPerPount = 16;

        public static decimal PoundsToOunces(decimal pounds)
        {
            return (pounds * OuncesPerPount);
        }

        public static decimal OuncesToPounds(decimal ounces)
        {
            return (ounces / OuncesPerPount);
        }

        public static int OuncesToWholePounds(decimal ounces)
        {
            return (int)Math.Floor(ounces / OuncesPerPount);
        }

        public static int OuncesToWholePoundsRemainder(decimal ounces)
        {
            return (int)(ounces - (OuncesToWholePounds(ounces) * OuncesPerPount));
        }

        public static decimal MillisecondsToSeconds(int milliseconds)
        {
            return (milliseconds / 1000);
        }

        public static int ConvertTime(this int value, TimeUnit from, TimeUnit to)
        {
            decimal converted = ConvertTime((decimal)value, from, to);

            if (from < to)
                return (int)Math.Floor(converted);
            else
                return (int)Math.Ceiling(converted);
        }

        public static decimal ConvertTime(this decimal value, TimeUnit from, TimeUnit to, int decimalPrecision = 2)
        {
            return Math.Round(value * ((decimal)from / (decimal)to), decimalPrecision);
        }
    }
}
