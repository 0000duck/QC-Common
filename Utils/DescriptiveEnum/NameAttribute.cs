using System;
using System.Collections.Generic;
using System.Text;

namespace QuantumConcepts.Common.Utils.DescriptiveEnum
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NameAttribute : System.Attribute
    {
        private string _name = null;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public NameAttribute() { }

        public NameAttribute(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
