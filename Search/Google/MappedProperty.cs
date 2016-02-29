using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace QuantumConcepts.Common.Search.Google
{
    public class MappedProperty
    {
        private string _name;
        private PropertyInfo _info;
        private bool _required;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public PropertyInfo Info
        {
            get { return _info; }
            set { _info = value; }
        }

        public bool Required
        {
            get { return _required; }
            set { _required = value; }
        }

        public MappedProperty() { }

        public MappedProperty(string name, PropertyInfo info)
        {
            _name = name;
            _info = info;
        }

        public MappedProperty(string name, PropertyInfo info, bool required)
        {
            _name = name;
            _info = info;
            _required = required;
        }
    }
}
