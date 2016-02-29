using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Export
{
    /// <summary>Defines a field to be exported. This includes a <see cref="Name"/> for the field and a <see cref="Selector"/> to extract the field's value.</summary>
    /// <typeparam name="T">The type of object within which this field resides.</typeparam>
    public class ExportFieldDefinition<T>
    {
        /// <summary>A friendly name for the field.</summary>
        public string Name { get; private set; }

        /// <summary>A selector to extract the field's value.</summary>
        public Func<T, object> Selector { get; private set; }

        /// <summary>Creates a new <see cref="ExportFieldDefinition&lt;T&gt;"/> using the provided <see cref="Name"/> and <see cref="Selector"/>.</summary>
        /// <param name="name"><see cref="Name"/></param>
        /// <param name="selector"><see cref="Selector"/></param>
        public ExportFieldDefinition(string name, Func<T, object> selector)
        {
            this.Name = name;
            this.Selector = selector;
        }
    }
}
