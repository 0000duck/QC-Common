using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QuantumConcepts.Common.Search
{
    public static class SearchUtil
    {
        public static void BreakApartKeywordString(string keywords, out List<string> include, out List<string> exclude)
        {
            if (string.IsNullOrEmpty(keywords))
            {
                include = null;
                exclude = null;
            }
            else
            {
                Regex regEx = new Regex("\"(?<Include>[^\"]+)\"|-(?<Exclude>[^\\s$]+)|(?<Include>[^\\s]+)");
                MatchCollection matches = regEx.Matches(keywords);

                include = new List<string>();
                exclude = new List<string>();

                foreach (Match m in matches)
                {
                    if (m.Groups["Include"] != null && !string.IsNullOrEmpty(m.Groups["Include"].Value.Trim()))
                        include.Add(m.Groups["Include"].Value);

                    if (m.Groups["Exclude"] != null && !string.IsNullOrEmpty(m.Groups["Exclude"].Value.Trim()))
                        exclude.Add(m.Groups["Exclude"].Value);
                }
            }
        }
    }
}
