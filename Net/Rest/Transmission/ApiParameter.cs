using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using QuantumConcepts.Common.Extensions;
using QuantumConcepts.Common.Utils;

namespace QuantumConcepts.Common.Net.Rest.Transmission
{
    public partial class ApiParameter : IApiParameter
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsValid { get { return (!string.IsNullOrEmpty(this.Key) && !string.IsNullOrEmpty(this.Value)); } }

        protected ApiParameter(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public static ApiParameter Create(string key, object value)
        {
            return new ApiParameter(key, value?.ToString());
        }

        public static ApiParameter Create(string key, int? value)
        {
            return new ApiParameter(key, (value.HasValue ? value.Value.ToString() : null));
        }

        public static ApiParameter Create(string key, long? value)
        {
            return new ApiParameter(key, (value.HasValue ? value.Value.ToString() : null));
        }

        public static ApiParameter Create(string key, decimal? value)
        {
            return new ApiParameter(key, (value.HasValue ? value.Value.ToString() : null));
        }

        public static ApiParameter Create(string key, decimal? value, string format)
        {
            return new ApiParameter(key, (value.HasValue ? value.Value.ToString(format) : null));
        }

        public static ApiParameter Create(string key, double? value)
        {
            return new ApiParameter(key, (value.HasValue ? value.Value.ToString() : null));
        }

        public static ApiParameter Create(string key, double? value, string format)
        {
            return new ApiParameter(key, (value.HasValue ? value.Value.ToString(format) : null));
        }

        public static ApiParameter Create(string key, DateTime? value)
        {
            return new ApiParameter(key, (value.HasValue ? value.Value.ToString("r") : null));
        }

        public static ApiParameter Create(string key, bool? value)
        {
            return new ApiParameter(key, (value.HasValue ? value.Value.ToString() : null));
        }

        public static ApiParameter Create(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                return new ApiParameter(key, value);

            return null;
        }

        public static ApiParameter Create(string key, params string[] values)
        {
            if (!values.IsNullOrEmpty())
                return new ApiParameter(key, string.Join(",", values));

            return null;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.Key) && !string.IsNullOrEmpty(this.Value))
                return (this.Key + "=" + WebUtility.UrlEncode(this.Value));

            return "";
        }
    }
}
