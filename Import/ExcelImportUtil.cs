using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Common.Import
{
    public static class ExcelImportUtil
    {
        public static OleDbConnection GetXlsConnection(string excelFilePath, bool readOnly = false)
        {
            return GetConnection(string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}; Extended Properties='Excel 12.0; ReadOnly={1}'", excelFilePath, readOnly));
        }

        public static OleDbConnection GetCsvConnection(string csvFilePath, bool firstLineIsColumnHeaders, string delimiter = ",", bool readOnly = false)
        {
            return GetConnection(string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}; Extended Properties='text; FMT=Delimited({1}); ReadOnly={2}; HDR={3}'", csvFilePath, delimiter, readOnly, (firstLineIsColumnHeaders ? "yes" : "no")));
        }

        public static OleDbConnection GetConnection(string connectionString)
        {
            OleDbConnection connection = new OleDbConnection(connectionString);

            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while connecting to the file: " + ex.Message, ex);
            }

            return connection;
        }

        public static string[] GetWorksheetNames(string excelFilePath)
        {
            DataTable tables = null;
            List<string> worksheetNames = new List<string>();

            using (OleDbConnection connection = GetXlsConnection(excelFilePath))
            {
                try
                {
                    tables = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new string[] { null, null, null, "TABLE" });
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("An error occurred while getting worksheet names from Excel file: " + ex.Message, ex);
                }
            }

            foreach (DataRow dr in tables.Rows)
                worksheetNames.Add((string)dr["TABLE_NAME"]);

            return worksheetNames.ToArray();
        }

        public static string[] GetColumnNamesForWorksheet(string excelFilePath, string worksheetName)
        {
            DataTable columns = null;
            List<string> columnNames = new List<string>();

            using (OleDbConnection connection = GetXlsConnection(excelFilePath))
            {
                try
                {
                    columns = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new string[] { null, null, worksheetName, null });
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("An error occurred while getting worksheet names from Excel file: " + ex.Message, ex);
                }
            }

            foreach (DataRow dr in columns.Rows)
                columnNames.Add((string)dr["COLUMN_NAME"]);

            return columnNames.ToArray();
        }

        public static DataTable GetDataTableFromWorksheet(string excelFilePath, string worksheetName, bool readOnly = false)
        {
            using (OleDbConnection connection = GetXlsConnection(excelFilePath, readOnly))
            {
                return GetDataTable(connection, "SELECT * FROM [" + worksheetName + "]");
            }
        }

        public static DataTable GetDataTableFromCsv(string csvFilePath, bool firstLineIsColumnHeaders, string delimiter, bool readOnly = false)
        {
            using (OleDbConnection connection = GetCsvConnection(Path.GetDirectoryName(csvFilePath), firstLineIsColumnHeaders, delimiter, readOnly))
            {
                return GetDataTable(connection, "SELECT * FROM [" + Path.GetFileName(csvFilePath) + "]");
            }
        }

        public static DataTable GetDataTable(OleDbConnection connection, string sql)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(sql, connection))
                {
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting data from file: " + ex.Message, ex);
            }

            return dataTable;
        }

        public static bool IsDataRowEmpty(DataRow dataRow)
        {
            foreach (DataColumn column in dataRow.Table.Columns)
                if (!dataRow.IsNull(column) && !string.IsNullOrEmpty(dataRow[column].ToString()))
                    return false;

            return true;
        }

        public static string ValidateAndConvertString(DataRow dr, string fieldName, bool required)
        {
            string value = Convert.ToString(dr[fieldName]);

            if (value.IsNullOrEmpty() && required)
                throw new ApplicationException("The field \"{0}\" is invalid. Expected type is string (required).".FormatString(fieldName));

            return value;
        }

        public static bool ValidateAndConvertBool(DataRow dr, string fieldName)
        {
            string valueString = Convert.ToString(dr[fieldName]);
            bool value;

            if (!bool.TryParse(valueString, out value))
                throw new ApplicationException("The field \"{0}\" is invalid. Expected type is boolean (true/false).".FormatString(fieldName));

            return value;
        }

        public static int ValidateAndConvertInt(DataRow dr, string fieldName)
        {
            string valueString = Convert.ToString(dr[fieldName]);
            int value;

            if (!int.TryParse(valueString, out value))
                throw new ApplicationException("The field \"{0}\" is invalid. Expected type is int.".FormatString(fieldName));

            return value;
        }

        public static decimal ValidateAndConvertDecimal(DataRow dr, string fieldName)
        {
            string valueString = Convert.ToString(dr[fieldName]);
            decimal value;

            valueString = valueString.Replace("$", "");

            if (!decimal.TryParse(valueString, out value))
                throw new ApplicationException("The field \"{0}\" is invalid. Expected type is decimal.".FormatString(fieldName));

            return value;
        }

        public static DateTime? ValidateAndConvertDateTime(DataRow dr, string fieldName, bool required)
        {
            string valueString = Convert.ToString(dr[fieldName]);
            DateTime value;

            //If there is a value, always ensure that it is valid.
            required = (required || !valueString.IsNullOrEmpty());

            if (!DateTime.TryParse(valueString, out value))
            {
                if (required)
                    throw new ApplicationException("The field \"{0}\" is invalid. Expected type is date/time (required) in the format: MM/dd/yyyy hh:mm:ss tt.".FormatString(fieldName));
                else
                    return null;
            }

            return value;
        }


    }
}
