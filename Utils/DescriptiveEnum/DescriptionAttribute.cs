using System;
using System.Collections.Generic;
using System.Text;

namespace QuantumConcepts.Common.Utils.DescriptiveEnum
{
    /// <summary>Empty interface meant to indicate an enum includes descriptions for each value.</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DescriptionAttribute : System.Attribute
    {
        private string _description = null;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public DescriptionAttribute() { }

        public DescriptionAttribute(string description)
        {
            _description = description;
        }

        public override string ToString()
        {
            return _description;
        }
    }
}
