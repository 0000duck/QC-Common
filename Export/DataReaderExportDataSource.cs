using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace QuantumConcepts.Common.Export
{
    public class DataReaderExportDataSource : IExportDataSource<object[]>
    {
        private IDataReader DataReader { get; set; }

        public ExportFieldDefinition<object[]>[] Fields { get; set; }
        public string Name { get; set; }

        public object[] Current
        {
            get
            {
                object[] values = new object[this.DataReader.FieldCount];

                this.DataReader.GetValues(values);

                return values;
            }
        }

        public DataReaderExportDataSource(IDataReader dataReader, string name)
        {
            if (dataReader == null)
                throw new ArgumentNullException("dataReader");

            DataTable schema = dataReader.GetSchemaTable();
            List<ExportFieldDefinition<object[]>> fields = new List<ExportFieldDefinition<object[]>>(schema.Columns.Count);

            foreach (DataRow row in schema.Rows)
            {
                string columnName = (string)row["ColumnName"];
                int ordinal = (int)row["ColumnOrdinal"];

                fields.Add(new ExportFieldDefinition<object[]>(columnName, o => o[ordinal]));
            }

            this.Fields = fields.ToArray();
            this.DataReader = dataReader;
            this.Name = name;
        }

        public bool MoveNext()
        {
            return this.DataReader.Read();
        }

        public void Dispose()
        {
            this.DataReader.Dispose();
        }
    }
}
