using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Utils
{
    public static class ExceptionUtil
    {
        public static string GetExceptionText(Exception exception)
        {
            return GetExceptionText(exception, "\n\n");
        }

        public static string GetExceptionText(Exception exception, string delimiter)
        {
            bool isFirst = true;
            StringBuilder text = new StringBuilder();
            Exception currentException = exception;

            while (currentException != null)
            {
                text.Append((isFirst ? "" : delimiter) + currentException.Message);

                currentException = currentException.InnerException;
                isFirst = false;
            }

            return text.ToString();
        }
    }
}
