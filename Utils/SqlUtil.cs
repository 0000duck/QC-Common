using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.IO;
using QuantumConcepts.Common.Extensions;
using System.Linq.Expressions;
using System.Data.Linq.Mapping;

namespace QuantumConcepts.Common.Utils
{
    public static class SqlUtil
    {
        public static SqlFileStream GetFileStream(SqlTransaction transaction, string tableName, string columnName, string primaryKeyColumnName, object primaryKeyValue, FileAccess fileAccess)
        {
            string sqlFilePath = null;
            byte[] transactionContext = null;

            using (SqlCommand command = new SqlCommand("SELECT [{0}].PathName() FROM {1} WHERE [{2}] = @primaryKeyValue".FormatString(columnName, tableName, primaryKeyColumnName), transaction.Connection, transaction))
            {
                command.Parameters.AddWithValue("primaryKeyValue", primaryKeyValue);
                sqlFilePath = (string)command.ExecuteScalar();
            }

            using (SqlCommand command = new SqlCommand("SELECT GET_FILESTREAM_TRANSACTION_CONTEXT()", transaction.Connection, transaction))
            {
                transactionContext = (byte[])command.ExecuteScalar();
            }

            return new SqlFileStream(sqlFilePath, transactionContext, fileAccess, FileOptions.SequentialScan, 0);
        }

        /// <summary>Creates a new VarBinaryStream by introspecting the provide type and lambda in order to determine the table and column names.</summary>
        public static SqlFileStream GetFileStream<T>(SqlTransaction transaction, T instance, Expression<Func<T, object>> columnSelector, Expression<Func<T, object>> primaryKeyColumnSelector, FileAccess fileAccess)
        {
            string tableName = GetTableName<T>();
            string columnName = GetColumnName<T>(instance, columnSelector);
            string primaryKeyColumnName = GetColumnName<T>(instance, primaryKeyColumnSelector);

            return GetFileStream(transaction, tableName, columnName, primaryKeyColumnName, primaryKeyColumnSelector.Compile().Invoke(instance), fileAccess);
        }

        public static string GetTableName<T>()
        {
            string name = typeof(T).GetAttribute<TableAttribute>().ValueOrDefault(a => a.Name);

            if (name.IsNullOrEmpty())
                throw new InvalidOperationException("Could not determine table name for type \"{0}\".".FormatString(typeof(T)));

            return name;
        }

        public static string GetColumnName<T>(this T instance, Expression<Func<T, object>> selector)
        {
            string name = typeof(T).GetProperty(selector.GetMemberName()).GetAttribute<ColumnAttribute>().ValueOrDefault(a => a.Name);

            if (name.IsNullOrEmpty())
                throw new InvalidOperationException("Could not determine column name for type \"{0}\".".FormatString(typeof(T)));

            return name;
        }
    }
}