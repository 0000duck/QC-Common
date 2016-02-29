using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumConcepts.Common.Net.Rest.Transmission;

namespace QuantumConcepts.Common.Net.Rest
{
    public static class Extensions
    {
        public static string BuildUrl(this IEnumerable<ApiParameter> parameters, string baseUrl)
        {
            if (parameters != null)
            {
                string parameterString = string.Join("&", parameters.Select(p => p.ToString()).ToArray());

                if (!string.IsNullOrEmpty(parameterString))
                    return baseUrl + "?" + parameterString;
            }

            return baseUrl;
        }
    }
}
