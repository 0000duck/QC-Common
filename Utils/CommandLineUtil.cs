using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace QuantumConcepts.Common.Utils
{
    public static class CommandLineUtil
    {
        public static Dictionary<string, string> SplitArguments(string[] args)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();

            if (args != null && args.Length > 0)
            {
                Regex argumentRegex = new Regex(@"^/(?<Key>[^=\r\n]+)(?:=(?<Value>[^\r\n]+))?.*$");

                foreach (string arg in args)
                {
                    Match match = argumentRegex.Match(arg);

                    if (match.Success)
                        arguments.Add(match.Groups["Key"].Value, match.Groups["Value"].Value);
                }
            }

            return arguments;
        }
    }
}
