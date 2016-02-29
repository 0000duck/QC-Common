using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumConcepts.Common.Utils;
using System.Text.RegularExpressions;

namespace QuantumConcepts.Common.Utils
{
    public static class TextUtil
    {
        public static string BuildVerificationKey()
        {
            Random random = new Random();
            char[] key = new char[21];

            for (int i = 0; i <= 20; i++)
                key[i] = Convert.ToChar(random.Next(65, 90));

            return new string(key, 0, 20);
        }

        public static string GetFullUnformattedPhoneNumber(string number, string extension)
        {
            string fullPhoneNumber = number + ("000000".Equals(extension) ? "" : "x" + extension);
            string unformattedPhoneNumber = RegexUtil.UnformattedPhoneNumber(fullPhoneNumber);

            if (string.IsNullOrEmpty(unformattedPhoneNumber))
                throw new Exception("The value '" + fullPhoneNumber + "' can not be formatted as a phone number.");

            return unformattedPhoneNumber;
        }

        public static void BreakApartKeywordString(string keywords, out List<string> include, out List<string> exclude)
        {
            Regex regEx = new Regex("\"(?<Include>[^\"]+)\"|-(?<Exclude>[^\\s$]+)|(?<Include>[^\\s]+)");
            MatchCollection matches = regEx.Matches(keywords);

            include = new List<string>();
            exclude = new List<string>();

            foreach (Match m in matches)
            {
                if (m.Groups["Include"] != null && !"".Equals(m.Groups["Include"].Value.Trim()))
                    include.Add(m.Groups["Include"].Value);

                if (m.Groups["Exclude"] != null && !"".Equals(m.Groups["Exclude"].Value.Trim()))
                    exclude.Add(m.Groups["Exclude"].Value);
            }
        }

        public static string GetExceptionText(Exception exception)
        {
            StringBuilder text = new StringBuilder();

            while (exception != null)
            {
                text.Append("\n\n" + exception.Message);
                exception = exception.InnerException;
            }

            return text.ToString();
        }
    }
}
