using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.DataObjects
{
    /// <summary>Represents a basic data object, requiring that the implementing class exposes an <see cref="IDataObject.ID"/> property of type <see cref="int"/>.</summary>
    public interface IDataObject : IDataObject<int> { }

    /// <summary>Represents a basic data object, requiring that the implementing class exposes an <see cref="IDataObject.ID"/> property of type <see cref="T"/>.</summary>
    /// <typeparam name="T">The type of the ID property.</typeparam>
    public interface IDataObject<T>
    {
        /// <summary>The internal or system ID of the data object.</summary>
        T ID { get; }
    }
}
