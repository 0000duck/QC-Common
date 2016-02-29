using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Data.Linq.Mapping;
using System.Linq.Expressions;
using QuantumConcepts.Common.Extensions;
using QuantumConcepts.Common.Utils;

namespace QuantumConcepts.Common.IO
{
    /// <summary>
    /// This class provides helper functions to retrieve data efficiently from a varbinary(max) column in the database. Specifically it is meant to help
    /// support functions that read and write to these columns without ever having the entire contents of the column in memory (as one large byte [])
    /// </summary>
    public static class SqlVarBinaryMaxHelper
    {
        /// <summary>
        /// This function can be used to read "part" of a varbinary(max) column
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="varBinaryColumnName"></param>
        /// <param name="primaryKeyColumnName"></param>
        /// <param name="primaryKeyValue"></param>
        /// <returns></returns>
        public static byte[] ReadPartOfVarbinaryColumn(SqlConnection connection, string tableName, string varBinaryColumnName, string primaryKeyColumnName, int primaryKeyValue, long startByte, long length)
        {
            string sqlTemplate = "SELECT SUBSTRING([{0}], @startByte, @length) AS [Data] FROM {1} WHERE [{2}] = @primaryKeyValue";

            string sql = string.Format(sqlTemplate, varBinaryColumnName, tableName, primaryKeyColumnName);

            using (SqlCommand command = connection.CreateCommand())
            {
                object result = null;

                command.CommandText = sql;
                command.Parameters.Add("@primaryKeyValue", SqlDbType.Int).Value = primaryKeyValue;
                command.Parameters.Add("@startByte", SqlDbType.BigInt).Value = (startByte + 1);
                command.Parameters.Add("@length", SqlDbType.BigInt).Value = length;

                result = command.ExecuteScalar();

                if (result == null || result is DBNull)
                    return null;
                else if (result is byte[])
                    return (byte[])result;

                throw new ApplicationException("Internal error: Wrong type returned in ReadPartOfVarbinaryColumn: " + result.GetType());
            }
        }

        /// <summary>
        /// This function can be used to determine the length (number of bytes) of a varbinary max column
        /// </summary>
        public static long GetLengthOfVarbinaryColumn(SqlConnection connection, string tableName, string varBinaryColumnName, string primaryKeyColumnName, object primaryKeyValue)
        {
            const string sqlTemplate = "SELECT LEN([{0}]) AS ColumnLength FROM {1} WHERE [{2}] = @primaryKeyValue";

            string sql = string.Format(sqlTemplate, varBinaryColumnName, tableName, primaryKeyColumnName);

            using (SqlCommand command = connection.CreateCommand())
            {
                object result = null;

                command.CommandText = sql;
                command.Parameters.Add("@primaryKeyValue", SqlDbType.Int).Value = primaryKeyValue;
                result = command.ExecuteScalar();

                //If the column doesn't have a length (it's null), then return 0.
                if (result == null || result is DBNull)
                    return 0;

                return Convert.ToInt64(result);
            }
        }

        /// <summary>
        /// This function can be used to determine the length (number of bytes) of a varbinary max column
        /// </summary>
        public static long GetLengthOfVarbinaryColumn<T>(SqlConnection connection, T instance, Expression<Func<T, object>> columnSelector, Expression<Func<T, object>> primaryKeyColumnSelector)
        {
            string tableName = SqlUtil.GetTableName<T>();
            string columnName = SqlUtil.GetColumnName<T>(instance, columnSelector);
            string primaryKeyColumnName = SqlUtil.GetColumnName<T>(instance, primaryKeyColumnSelector);

            return GetLengthOfVarbinaryColumn(connection, tableName, columnName, primaryKeyColumnName, primaryKeyColumnSelector.Compile().Invoke(instance));
        }

        /// <summary>
        /// This column will write read the contents of the specified file and write them to the column specified in the dbTableName and dbColumnName
        /// attributes.
        /// </summary>
        public static void UpdateColumnWithFile(SqlConnection connection, string fileName, string tableName, string varBinaryColumnName, string primaryKeyColumnName, int primaryKeyValue)
        {
            // Open the file 
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                long nextByte = 0;
                bool keepReading = true;

                while (keepReading)
                {
                    // Read from the file 1MB at a time
                    const int BYTES_TO_READ_AT_TIME = 1000000;
                    byte[] byteBuffer = new byte[BYTES_TO_READ_AT_TIME];
                    int bytesRead = fileStream.Read(byteBuffer, 0, BYTES_TO_READ_AT_TIME);

                    if (bytesRead <= 0)
                    {
                        keepReading = false;
                    }
                    else
                    {
                        byte[] bytesToWrite = null;

                        if (bytesRead == BYTES_TO_READ_AT_TIME)
                            bytesToWrite = byteBuffer;
                        else
                        {
                            bytesToWrite = new byte[bytesRead];
                            Array.Copy(byteBuffer, bytesToWrite, bytesRead);
                        }

                        // Now let's write this to the database...
                        nextByte += WritePartOfVarBinaryColumn(connection, tableName, varBinaryColumnName, primaryKeyColumnName, primaryKeyValue, nextByte, bytesToWrite);
                    }
                }
            }
        }

        /// <summary>Initialized the column's value to 0x0 so it may be written.</summary>
        public static void InitializeVarBinaryColumn(SqlConnection connection, string tableName, string varBinaryColumnName, string primaryKeyColumnName, object primaryKeyValue)
        {
            const string sqlTemplate = "UPDATE {0} SET [{1}] = 0x WHERE [{2}] = @PrimaryKeyValue";

            string sql = string.Format(sqlTemplate, tableName, varBinaryColumnName, primaryKeyColumnName);

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddWithValue("@PrimaryKeyValue", primaryKeyValue);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(string.Format("Unable to initialize VarBinary column {0}.{1}.", tableName, varBinaryColumnName), ex);
                }
            }
        }

        /// <summary>Initialized the column's value to 0x0 so it may be written.</summary>
        public static void InitializeVarBinaryColumn<T>(SqlConnection connection, T instance, Expression<Func<T, object>> columnSelector, Expression<Func<T, object>> primaryKeyColumnSelector)
        {
            string tableName = SqlUtil.GetTableName<T>();
            string columnName = SqlUtil.GetColumnName<T>(instance, columnSelector);
            string primaryKeyColumnName = SqlUtil.GetColumnName<T>(instance, primaryKeyColumnSelector);

            InitializeVarBinaryColumn(connection, tableName, columnName, primaryKeyColumnName, primaryKeyColumnSelector.Compile().Invoke(instance));
        }

        /// <summary>Writes the supplied buffer to the column indicated.</summary>
        /// <returns>The length of data that was written.</returns>
        public static long WritePartOfVarBinaryColumn(SqlConnection connection, string tableName, string varBinaryColumnName, string primaryKeyColumnName, int primaryKeyValue, long offset, byte[] buffer)
        {
            const string sqlTemplate = "UPDATE {0} SET [{1}].WRITE(@buffer, @offset, @length) WHERE [{2}] = @primaryKeyValue";

            string sql = string.Format(sqlTemplate, tableName, varBinaryColumnName, primaryKeyColumnName);
            long length = buffer.Length;

            using (SqlCommand dbCommand = connection.CreateCommand())
            {
                dbCommand.CommandText = sql;
                dbCommand.Parameters.Add("@primaryKeyValue", SqlDbType.Int).Value = primaryKeyValue;
                dbCommand.Parameters.Add("@offset", SqlDbType.BigInt).Value = offset;
                dbCommand.Parameters.Add("@length", SqlDbType.BigInt).Value = length;
                dbCommand.Parameters.Add("@buffer", SqlDbType.VarBinary).Value = buffer;

                dbCommand.ExecuteNonQuery();
            }

            return length;
        }
    }
}