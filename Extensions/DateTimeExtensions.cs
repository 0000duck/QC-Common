using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumConcepts.Common.Utils;

namespace QuantumConcepts.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool IsInRange(this DateTime effectiveDate, DateTime? start, DateTime? end)
        {
            if ((!start.HasValue || start.Value <= effectiveDate.Date) && (!end.HasValue || end.Value >= effectiveDate.Date))
                return true;

            return false;
        }

        /// <summary>
        ///     Returns a string representation of the DateTime: Nov 1, 2008 at 10:44 PM.
        /// </summary>
        public static string ToDisplayString(this DateTime dateTime)
        {
            return dateTime.ToDisplayString(true);
        }

        /// <summary>
        ///     Returns a string representation of the DateTime: Nov 1, 2008 at 10:44 PM.
        /// </summary>
        public static string ToDisplayString(this DateTime dateTime, bool includeTime)
        {
            return dateTime.ToString("MMM d, yyyy" + (includeTime ? " a\\t h:mm tt" : ""));
        }

        /// <summary>Determines whether or not the date falls on a weekend (Saturday or Sunday).</summary>
        public static bool IsWeekend(this DateTime dateTime)
        {
            return (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday);
        }

        /// <summary>Determines whether or not the date falls Monday through Friday.</summary>
        public static bool IsWorkingDay(this DateTime dateTime)
        {
            return !dateTime.IsWeekend();
        }

        /// <summary>
        ///     Returns a string representation of the TimeSpan: 0d 1h 34m 44s 123ms.
        /// </summary>
        public static string ToShortestDisplayString(this TimeSpan timeSpan)
        {
            return ToShortestDisplayString(timeSpan, true, true, true, true, true);
        }

        /// <summary>
        ///     Returns a string representation of the TimeSpan: 0d 1h 34m 44s 123ms.
        /// </summary>
        public static string ToShortestDisplayString(this TimeSpan timeSpan, bool includeDays, bool includeHours, bool includeMinutes, bool includeSeconds, bool includeMilliseconds)
        {
            StringBuilder display = new StringBuilder();

            if (includeDays && timeSpan.Days > 0)
                display.Append(timeSpan.Days + "d ");

            if (includeHours && timeSpan.Hours > 0)
                display.Append(timeSpan.Hours + "h ");

            if (includeMinutes && timeSpan.Minutes > 0)
                display.Append(timeSpan.Minutes + "m ");

            if (includeSeconds && timeSpan.Seconds > 0)
                display.Append(timeSpan.Seconds + "s ");

            if (includeMilliseconds)
                display.Append(timeSpan.Milliseconds + "ms ");

            return display.ToString().Trim();
        }

        public static string ToDisplayString(this TimeSpan timeSpan, bool includeSeconds, bool includeAmPm)
        {
            bool am = (timeSpan.Hours < 12);
            int hour = (timeSpan.Hours == 0 ? 12 : (timeSpan.Hours > 12 ? timeSpan.Hours - 12 : timeSpan.Hours));

            if (includeSeconds)
                return "{0}:{1}:{2} {3}".FormatString(hour, timeSpan.Minutes.ToString().PadLeft(2, '0'), timeSpan.Seconds.ToString().PadLeft(2, '0'), (includeAmPm ? (am ? "AM" : "PM") : "")).Trim();
            else
                return "{0}:{1} {2}".FormatString(hour, timeSpan.Minutes.ToString().PadLeft(2, '0'), (includeAmPm ? (am ? "AM" : "PM") : "")).Trim();
        }

        /// <summary>Returns a DateTime using today's date and the provided TimeSpan for the time.</summary>
        public static DateTime ToDateTime(this TimeSpan timeSpan)
        {
            DateTime now = DateTimeUtil.Now;

            return timeSpan.ToDateTime(now.Year, now.Month, now.Day);
        }

        /// <summary>Returns a DateTime using the provided year, month, and day for the date and the provided TimeSpan for the time.</summary>
        public static DateTime ToDateTime(this TimeSpan timeSpan, int year, int month, int day)
        {
            return new DateTime(year, month, day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        }

        /// <summary>Creates a new date based on the provided date but with the milliseconds set to 0.</summary>
        /// <param name="dateTime">The DateTime to clean.</param>
        /// <returns>A new instance of the specified DateTime but with the milliseconds set to 0.</returns>
        public static DateTime ToWholeMinutes(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
        }

        public static string ToLargestWholeDisplayString(this TimeSpan timeSpan)
        {
            TimeSpan largest = timeSpan.ToLargestWholeValue();
            StringBuilder display = new StringBuilder();

            if (largest.Days > 0)
                display.Append(timeSpan.Days + "d ");

            if (largest.Hours > 0)
                display.Append(timeSpan.Hours + "h ");

            if (largest.Minutes > 0)
                display.Append(timeSpan.Minutes + "m ");

            if (largest.Seconds > 0)
                display.Append(timeSpan.Seconds + "s ");

            if (largest.Milliseconds > 0)
                display.Append(timeSpan.Milliseconds + "ms ");

            return display.ToString().Trim();
        }

        public static TimeSpan ToLargestWholeValue(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
                return new TimeSpan((int)Math.Floor(timeSpan.TotalDays), 0, 0, 0);
            else if (timeSpan.TotalHours >= 1)
                return new TimeSpan(0, (int)Math.Floor(timeSpan.TotalHours), 0, 0);
            else if (timeSpan.TotalMinutes >= 1)
                return new TimeSpan(0, 0, (int)Math.Floor(timeSpan.TotalMinutes), 0);
            else if (timeSpan.TotalSeconds >= 1)
                return new TimeSpan(0, 0, 0, (int)Math.Floor(timeSpan.TotalSeconds));
            else
                return new TimeSpan(0, 0, 0, 0, (int)Math.Floor(timeSpan.TotalMilliseconds));
        }

        public static DateTime ToStartOfWeek(this DateTime dateTime)
        {
            return dateTime.AddDays(-(int)dateTime.DayOfWeek);
        }

        public static DateTime ToEndOfWeek(this DateTime dateTime)
        {
            return dateTime.ToStartOfWeek().AddDays(6);
        }

        public static DateTime CleanMilliseconds(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
        }
    }
}
