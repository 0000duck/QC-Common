using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.Linq.Mapping;
using System.Linq.Expressions;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Common.IO
{
    /// <summary>Facilitates reading and writing a VarBinary column as a byte stream.</summary>
    public class SqlVarBinaryStream : Stream
    {
        private const long BufferSize = (10 * 1024 * 1024); //10MB

        private SqlConnection Connection { get; set; }
        private string TableName { get; set; }
        private string ColumnName { get; set; }
        private string PrimaryKeyColumnName { get; set; }
        private int PrimaryKeyValue { get; set; }

        private bool? IsReading { get; set; }
        private long LengthInternal { get; set; }
        private byte[] Buffer { get; set; }
        private int BufferPosition { get; set; }
        private bool ClearData { get; set; }

        public override bool CanRead { get { return (!this.IsReading.HasValue || this.IsReading.Value); } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return (!this.IsReading.HasValue || !this.IsReading.Value); } }
        public override long Length { get { return this.LengthInternal; } }
        public override long Position { get { return this.BufferPosition; } set { throw new NotSupportedException("Seeking is not supported in this stream."); } }

        private SqlVarBinaryStream(SqlConnection connection, string tableName, string columnName, string primaryKeyColumnName, int primaryKeyValue)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("tableName");

            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException("columnName");

            this.Connection = connection;
            this.TableName = tableName;
            this.ColumnName = columnName;
            this.PrimaryKeyColumnName = primaryKeyColumnName;
            this.PrimaryKeyValue = primaryKeyValue;
            this.LengthInternal = SqlVarBinaryMaxHelper.GetLengthOfVarbinaryColumn(this.Connection, this.TableName, this.ColumnName, this.PrimaryKeyColumnName, this.PrimaryKeyValue);
            this.ClearData = true;
        }

        /// <summary>Creates a new VarBinaryStream based on the provided parameters.</summary>
        public static SqlVarBinaryStream CreateVarBinaryStream(SqlConnection connection, string tableName, string columnName, string primaryKeyColumnName, int primaryKeyValue)
        {
            return new SqlVarBinaryStream(connection, tableName, columnName, primaryKeyColumnName, primaryKeyValue);
        }

        /// <summary>Creates a new VarBinaryStream by introspecting the provide type and lambda in order to determine the table and column names.</summary>
        public static SqlVarBinaryStream CreateVarBinaryStream<T>(SqlConnection connection, T instance, Expression<Func<T, byte[]>> columnSelector, Expression<Func<T, int>> primaryKeyColumnSelector)
        {
            Type typeOfT = typeof(T);
            string tableName = null;
            string columnName = null;
            string primaryKeyColumnName = null;

            tableName = typeOfT.GetAttribute<TableAttribute>().ValueOrDefault(a => a.Name);
            columnName = typeOfT.GetProperty(columnSelector.GetMemberName()).GetAttribute<ColumnAttribute>().ValueOrDefault(a => a.Name);
            primaryKeyColumnName = typeOfT.GetProperty(primaryKeyColumnSelector.GetMemberName()).GetAttribute<ColumnAttribute>().ValueOrDefault(a => a.Name);

            if (tableName.IsNullOrEmpty())
                throw new InvalidOperationException("Could not determine table name for type \"{0}\".".FormatString(typeOfT.Name));

            if (columnName.IsNullOrEmpty())
                throw new InvalidOperationException("Could not determine column name for type \"{0}\".".FormatString(typeOfT.Name));

            if (primaryKeyColumnName.IsNullOrEmpty())
                throw new InvalidOperationException("Could not determine primary key column name for type \"{0}\".".FormatString(typeOfT.Name));

            return new SqlVarBinaryStream(connection, tableName, columnName, primaryKeyColumnName, primaryKeyColumnSelector.Compile().Invoke(instance));
        }

        /// <summary>Writes the contents of the buffer to the VarBinary column and clears the buffer.</summary>
        public override void Flush()
        {
            //If we're in read mode (or no mode), then we don't want to flush the data.
            if (!this.IsReading.HasValue || this.IsReading.Value)
                return;

            //If there is nothing to flush then don't do anything.
            if (this.BufferPosition == 0)
                return;

            byte[] bytesToWrite = new byte[this.BufferPosition];

            //Copy from the buffer.
            System.Buffer.BlockCopy(Buffer, 0, bytesToWrite, 0, this.BufferPosition);

            //Write the bytes to the column and update the position.
            this.Position += SqlVarBinaryMaxHelper.WritePartOfVarBinaryColumn(this.Connection, this.TableName, this.ColumnName, this.PrimaryKeyColumnName, this.PrimaryKeyValue, this.Position, bytesToWrite);

            //Clear the buffer.
            this.Buffer = new byte[SqlVarBinaryStream.BufferSize];
            this.BufferPosition = 0;
        }

        /// <summary>Reads data from the VarBinary column into the supplied buffer.</summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!this.IsReading.HasValue)
                this.IsReading = true;
            else if (!this.IsReading.Value)
                throw new InvalidOperationException("The stream is already being used to write data; it cannot be used to read data as well.");

            int bytesToWrite = count;
            int bytesWritten = 0;

            do
            {
                int bytesToCopy = 0;

                //If the buffer is empty, read another chunk of data.
                if (Buffer == null || (this.Buffer.Length - this.BufferPosition) == 0)
                {
                    byte[] newBytes = SqlVarBinaryMaxHelper.ReadPartOfVarbinaryColumn(this.Connection, this.TableName, this.ColumnName, this.PrimaryKeyColumnName, this.PrimaryKeyValue, this.Position, SqlVarBinaryStream.BufferSize);
                    int bytesRead = (newBytes == null ? 0 : newBytes.Length);

                    if (bytesRead > 0)
                    {
                        this.Buffer = new byte[newBytes.Length];
                        this.BufferPosition = 0;
                        newBytes.CopyTo(Buffer, 0);
                        this.Position += bytesRead;
                    }
                    else
                        break;
                }

                //Copy to the output buffer.
                bytesToCopy = System.Math.Min(bytesToWrite, this.Buffer.Length - this.BufferPosition);
                System.Buffer.BlockCopy(this.Buffer, this.BufferPosition, buffer, offset + bytesWritten, bytesToCopy);

                //Update the counters.
                bytesToWrite -= bytesToCopy;
                bytesWritten += bytesToCopy;
                this.BufferPosition += bytesToCopy;
            } while (bytesWritten < count);

            return bytesWritten;
        }

        /// <summary>Writes data to the VarBinary column from the supplied buffer.</summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!this.IsReading.HasValue)
                this.IsReading = false;
            else if (this.IsReading.Value)
                throw new InvalidOperationException("The stream is already being used to read data; it cannot be used to write data as well.");

            int bytesToWrite = count;
            int bytesWritten = 0;

            //On the first write, clear the data from the column.
            if (this.ClearData)
                this.ClearColumnData();

            //If the buffer has not been allocated, do so now.
            if (this.Buffer == null)
                this.Buffer = new byte[SqlVarBinaryStream.BufferSize];

            do
            {
                int bytesToCopy = System.Math.Min(bytesToWrite, this.Buffer.Length - this.BufferPosition);

                System.Buffer.BlockCopy(buffer, offset + bytesWritten, this.Buffer, this.BufferPosition, bytesToCopy);
                bytesToWrite -= bytesToCopy;
                bytesWritten += bytesToCopy;
                this.BufferPosition += bytesToCopy;

                if (this.BufferPosition == this.Buffer.Length)
                    this.Flush();
            } while (bytesToWrite > 0);
        }

        /// <summary>Sets the column's data to 0x0. Otherwise, if you were to write a smaller file to the column than the existing data,the existing data would still remain after the smaller data is written.</summary>
        private void ClearColumnData()
        {
            //Initialize (clear) the column.
            SqlVarBinaryMaxHelper.InitializeVarBinaryColumn(this.Connection, this.TableName, this.ColumnName, this.PrimaryKeyColumnName, this.PrimaryKeyValue);

            //Indicate that the data no longer needs to be cleared.
            this.ClearData = false;
        }

        /// <summary>Seeking is not supported in this stream.</summary>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("Seeking is not supported in this stream.");
        }

        /// <summary>Seeking is not supported in this stream.</summary>
        public override void SetLength(long value)
        {
            throw new NotSupportedException("Seeking is not supported in this stream.");
        }

        protected override void Dispose(bool disposing)
        {
            if (this.IsReading.HasValue && !this.IsReading.Value)
                this.Flush();

            base.Dispose(disposing);
        }
    }
}
