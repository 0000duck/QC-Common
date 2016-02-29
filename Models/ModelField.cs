using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Models
{
    public class ModelField<FT>
    {
        public string Name { get; private set; }
        public FT Value { get; set; }

        public ModelField(string name) : this(name, default(FT)) { }

        public ModelField(string name, FT value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}