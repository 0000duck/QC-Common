using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Search.Google
{
    public class ParameterValue : IParameterValue
    {
        private string _value;
        private bool _exclude;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public bool Exclude
        {
            get { return _exclude; }
            set { _exclude = value; }
        }

        public override string ToString()
        {
            return "(" + ((_exclude ? "-" : "") + _value) + ")";
        }
    }
}
