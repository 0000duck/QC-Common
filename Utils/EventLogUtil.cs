using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Common.Utils
{
    public static class EventLogUtil
    {
        public static void WriteLog(string source, Exception ex, EventLogEntryType type)
        {
            WriteLog(source, ex.ToString(), type);
        }

        public static void WriteLog(string source, string message, EventLogEntryType type)
        {
            const string applicationLog = "Application";

            try
            {
                if (!EventLog.SourceExists(source))
                    EventLog.CreateEventSource(source, applicationLog);

                EventLog.WriteEntry(source, message, type);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to access Event Log.", ex);
            }
        }
    }
}