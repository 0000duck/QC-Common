using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QuantumConcepts.Common.Extensions {
    public static class StringExtensions
    {
        /// <summary>Determines if both strings are equal, regardless of case.</summary>
        public static bool EqualsIgnoreCase(this string a, string b)
        {
            return string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase);
        }

        public static string FormatString(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static void Parse(this List<string> strings, string keywords)
        {
            if (string.IsNullOrEmpty(keywords))
            {
                return;
            }

            string[] strKeywords = keywords.Split(',');

            strings.Clear();

            foreach (var str in strKeywords)
            {
                strings.Add(str);
            }
        }

        public static string TryTrim(this string value)
        {
            return value?.Trim();
        }

        public static string ToCommaDelimitedString(this List<string> strings)
        {
            StringBuilder keywords = new StringBuilder();

            foreach (string str in strings)
            {
                keywords.Append(str);

                if (str != strings[strings.Count - 1])
                    keywords.Append(",");
            }

            return keywords.ToString();
        }

        public static bool IsContainedWithin(this string textToFind, string textToSearch)
        {
            if (textToFind == null || string.IsNullOrEmpty(textToSearch))
                return false;

            return textToSearch.ToLower().Contains(textToFind.ToLower());
        }

        public static bool IsContainedWithinAny(this string textToFind, params string[] textToSearch)
        {
            if (textToFind == null || textToSearch == null)
            {
                return false;
            }

            return textToSearch.Any(s => textToFind.IsContainedWithin(s));
        }

        public static bool IsContainedWithinAll(this string textToFind, params string[] textToSearch)
        {
            if (textToFind == null || textToSearch == null)
            {
                return false;
            }

            return textToSearch.All(s => textToFind.IsContainedWithin(s));
        }

        public static string StripHtml(this string text)
        {
            if (text.IsNullOrEmpty())
            {
                return null;
            }

            return Regex.Replace(text, "\\<[^>]+>", "");
        }

        /// <summary>Trims a block of text at the given length and appends an ellipsis if more characters exist.</summary>
        /// <param name="text">The text to trim.</param>
        /// <param name="length">The length of the text.</param>
        /// <param name="breakOnWholeWord">Whether or not to break on the next space instead of in the middle of a word.</param>
        /// <returns>The trimmed text.</returns>
        public static string TrimTextBlock(this string text, int length, bool breakOnWholeWord)
        {
            return TrimTextBlock(text, length, breakOnWholeWord, null);
        }

        /// <summary>Trims a block of text at the given length and appends an ellipsis if more characters exist.</summary>
        /// <param name="text">The text to trim.</param>
        /// <param name="length">The length of the text.</param>
        /// <param name="breakOnWholeWord">Whether or not to break on the next space instead of in the middle of a word.</param>
        /// <param name="moreLinkDelegate">A delegate which returns a link to more information.</param>
        /// <returns>The trimmed text.</returns>
        public static string TrimTextBlock(this string text, int length, bool breakOnWholeWord, GetStringValueDelegate moreLinkDelegate)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= length)
            {
                return text;
            }
            else
            {
                int trimLength = (breakOnWholeWord ? text.IndexOf(" ", length) : length);
                string trimmedText = null;

                if (trimLength < 0 || trimLength > text.Length)
                {
                    trimmedText = text;
                }
                else
                {
                    trimmedText = text.Substring(0, trimLength);
                }

                return "{0}...{1}".FormatString(trimmedText, (moreLinkDelegate == null ? "" : moreLinkDelegate()));
            }
        }

        public static void BreakApartKeywordString(this string keywords, out List<string> include, out List<string> exclude)
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
    }
}
