using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Common.Security.OAuth
{
    public class OAuthParameter : IComparable<OAuthParameter>
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public OAuthParameter(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public override string ToString()
        {
            return "{0}={1}".FormatString(this.Key, this.Value);
        }

        public int CompareTo(OAuthParameter other)
        {
            if (other == null)
                return -1;

            if (string.Equals(this.Key, other.Key))
                return string.Compare(this.Value, other.Value);
            else
                return string.Compare(this.Key, other.Key);
        }
    }
}
