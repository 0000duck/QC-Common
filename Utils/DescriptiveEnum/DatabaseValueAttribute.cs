using System;
using System.Collections.Generic;
using System.Text;

namespace QuantumConcepts.Common.Utils.DescriptiveEnum
{
    /// <summary>Empty interface meant to indicate an enum includes descriptions for each value.</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DatabaseValueAttribute : System.Attribute
    {
        private string _databaseValue = null;

        public string DatabaseValue
        {
            get { return _databaseValue; }
            set { _databaseValue = value; }
        }

        public DatabaseValueAttribute() { }

        public DatabaseValueAttribute(string databaseValue)
        {
            _databaseValue = databaseValue;
        }

        public override string ToString()
        {
            return _databaseValue;
        }
    }
}
