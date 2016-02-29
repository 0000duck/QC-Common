using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Export
{
    /// <summary>Defines a datasource which is comprised of a set of <see cref="ExportFieldDefinition"/>s.</summary>
    /// <typeparam name="T"></typeparam>
    public class EnumerableExportDataSource<T> : IExportDataSource<T>, IEnumerable<T>, IEnumerator<T>
    {
        private IEnumerable<T> DataSource { get; set; }
        private IEnumerator<T> Enumerator { get; set; }
        object System.Collections.IEnumerator.Current { get { return this.Enumerator.Current; } }

        /// <summary>The fields that will be extracted from each item.</summary>
        public ExportFieldDefinition<T>[] Fields { get; set; }

        /// <summary>The item which is currently "selected" within the <see cref="IEnumerator&lt;T&gt;"/>.</summary>
        public T Current { get { return this.Enumerator.Current; } }

        /// <summary>A friendly name for the data source.</summary>
        public string Name { get; set; }

        /// <summary>Creates a new <see cref="EnumerableExportDataSource&lt;T&gt;"/> using the provided <see cref="Fields"/>, data source and <see cref="Name"/>.</summary>
        /// <param name="fields"><see cref="Fields"/></param>
        /// <param name="dataSource">The data source which will be iterated to extract data from each item.</param>
        /// <param name="name"><see cref="Name"/></param>
        /// <exception cref="ArgumentNullException">Throw when the parameter fields or dataSource are null.</exception>
        public EnumerableExportDataSource(ExportFieldDefinition<T>[] fields, IEnumerable<T> dataSource, string name)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            if (dataSource == null)
                throw new ArgumentNullException("dataSource");

            this.Fields = fields;
            this.DataSource = dataSource;
            this.Enumerator = this.DataSource.GetEnumerator();
            this.Name = name;
        }
        /// <summary>Advances the enumerator to the next element of the collection.</summary>
        public bool MoveNext()
        {
            return this.Enumerator.MoveNext();
        }

        /// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
        public void Reset()
        {
            this.Enumerator.Reset();
        }

        /// <summary>Disposes the underlying <see cref="IEnumerator"/>.</summary>
        public void Dispose()
        {
            this.Enumerator.Dispose();
        }

        /// <summary>Returns an <see cref="IEnumerator&lt;T&gt;"/> which iterates over the data source.</summary>
        public IEnumerator<T> GetEnumerator()
        {
            return this.DataSource.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.DataSource.GetEnumerator();
        }
    }
}
