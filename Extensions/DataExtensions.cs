using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.ComponentModel;
using System.Data;

namespace QuantumConcepts.Common.Extensions
{
    public static class DataExtensions
    {
        public static T TryGetValue<T>(this DbDataReader dataReader, string fieldName)
        {
            object value = dataReader[fieldName];

            if (value == null || value == DBNull.Value)
                return default(T);

            return (T)value;
        }

        public static T TryGetValue<T>(this DataRow dataRow, string columnName)
        {
            return dataRow.TryGetValue<T>(dataRow.Table.Columns.IndexOf(dataRow.Table.Columns[columnName]));
        }

        public static T TryGetValue<T>(this DataRow dataRow, int columnIndex)
        {
            if (columnIndex >= 0 && columnIndex < dataRow.Table.Columns.Count)
            {
                object value = dataRow[columnIndex];

                if (value != null && value != DBNull.Value)
                    return (T)value;
            }

            return default(T);
        }
    }
}
