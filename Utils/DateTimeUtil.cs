using System;

namespace QuantumConcepts.Common.Utils
{
    /// <summary>Provides a means to "fake" the current date. This is useful when creating test data.</summary>
    public static class DateTimeUtil
    {
        private static TimeSpan? Offset { get; set; }

        /// <summary>The Current date and time - this could be the actual date and time or the modified date and time if Set() has been called.</summary>
        public static DateTime Now { get { return (DateTimeUtil.Offset.HasValue ? DateTime.Now.Subtract(DateTimeUtil.Offset.Value) : DateTimeUtil.Actual); } }

        /// <summary>The Actual date and time - this will be equal to the CurrentDateTime if no offset has been set.</summary>
        public static DateTime Actual { get { return DateTime.Now; } }

        /// <summary>Indicates whether or not CurrentDateTime is currently offset. If IsOffset is false, then CurrentDateTime is the ActualDateTime.</summary>
        public static bool IsOffset { get { return DateTimeUtil.Offset.HasValue && DateTimeUtil.Offset.Value.TotalMilliseconds == 0; } }

        /// <summary>Sets the offset by which the CurrentDateTime is calculated.</summary>
        /// <param name="dateTime">The date and time you wish it to be now. This will be used as n offset, not a static date and time.</param>
        public static void Set(DateTime dateTime)
        {
            DateTimeUtil.Offset = DateTime.Now.Subtract(dateTime);
        }

        /// <summary>Resets the CurrentDateTime to the actual date and time by clearing the offset.</summary>
        public static void Reset()
        {
            DateTimeUtil.Offset = null;
        }
    }
}
