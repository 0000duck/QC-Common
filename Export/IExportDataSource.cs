using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Export
{
    public interface IExportDataSource<T>
    {
        ExportFieldDefinition<T>[] Fields { get; }

        T Current { get; }

        bool MoveNext();

        /// <summary>
        /// The name of the data source. 
        /// </summary>
        string Name { get; set;} 
    }
}
