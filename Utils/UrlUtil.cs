using QuantumConcepts.Common.Extensions;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QuantumConcepts.Common.Utils
{
    public partial class UrlUtil
    {
        private static readonly char[] ReservedCharacters = { '!', '*', '\'', '(', ')', ';', ':', '@', '&', '=', '+', '$', ',', '/', '?', '%', '#', '[', ']', ' ' };
        private static readonly char[] NonReservedCharacters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_', '.', '~' };

        /// <summary>Replaces all characters which are spaces with "-" and all other characters which are not letters, numbers, or hyphens with nothing.</summary>
        /// <param name="text">The text to format.</param>
        /// <returns>A friendly URL-safe version of text.</returns>
        public static string FormatForUrl(string text)
        {
            string formattedText = text;

            formattedText = Regex.Replace(formattedText, @"\s", "-");
            formattedText = Regex.Replace(formattedText, @"[^a-z0-9\-]", "", RegexOptions.IgnoreCase);

            return formattedText;
        }

        /// <summary>This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case. While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth.</summary>
        /// <param name="value">The value to Url encode.</param>
        /// <returns>Returns a Url encoded string'</returns>
        public static string UrlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            StringBuilder encoded = new StringBuilder();

            foreach (char c in value)
            {
                if (ReservedCharacters.Contains(c))
                {
                    encoded.Append("%{0:X2}".FormatString((int)c));
                }
                else if (!NonReservedCharacters.Contains(c))
                {
                    throw new ApplicationException("Character '{0}' is not valid.".FormatString(c));
                }
                else
                {
                    encoded.Append(c);
                }
            }

            return encoded.ToString();
        }
    }
}