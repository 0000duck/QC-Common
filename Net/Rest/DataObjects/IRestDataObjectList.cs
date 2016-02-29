using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Net.Rest.DataObjects
{
    public interface IRestDataObjectList<T> : IRestDataObject
    {
        List<T> InnerList { get; set; }
    }
}
