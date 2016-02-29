using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;

namespace QuantumConcepts.Common.Utils
{
    public static class ImportUtil
    {
        public enum ColumnAggregationMode { None, Numeric, Boolean }

        private static string[] GetColumnNames(string columnName)
        {
            Regex regex = new Regex(@"(?<ColumnName>[^+|]+)(\+|\|)?");
            MatchCollection matches = regex.Matches(columnName);

            if (matches.Count > 0)
            {
                List<string> columnNames = new List<string>();

                foreach (Match match in matches)
                    if (match.Groups["ColumnName"].Success)
                        columnNames.Add(match.Groups["ColumnName"].Value);

                return columnNames.ToArray();
            }

            return new string[] { columnName };
        }

        public static string GetValueFromDataRow(DataRow dr, string columnName)
        {
            return GetValueFromDataRow(dr, columnName, ColumnAggregationMode.None);
        }

        public static string GetValueFromDataRow(DataRow dr, string columnName, ColumnAggregationMode aggregationMode)
        {
            if (string.IsNullOrEmpty(columnName))
                return null;

            string[] columnNames = GetColumnNames(columnName);
            string[] values = GetValuesFromDataRow(dr, columnName);

            if (values == null || values.Length == 0)
                return null;
            else if (values.Length == 1)
            {
                return values[0];
            }
            else if (aggregationMode == ColumnAggregationMode.Numeric)
            {
                decimal value = 0M;

                foreach (string s in values)
                {
                    decimal tempDecimal;

                    if (decimal.TryParse(s, out tempDecimal))
                        value += tempDecimal;
                }

                return value.ToString();
            }
            else if (aggregationMode == ColumnAggregationMode.Boolean)
            {
                bool value = false;

                foreach (string s in values)
                {
                    bool tempBool = false;

                    if ((bool.TryParse(s, out tempBool) && tempBool) || "yes".Equals(s, StringComparison.InvariantCultureIgnoreCase))
                    {
                        value = true;
                        break;
                    }
                }

                return value.ToString();
            }

            return null;
        }

        public static string[] GetValuesFromDataRow(DataRow dr, string columnName)
        {
            string[] columnNames = GetColumnNames(columnName);

            if (columnNames.Length == 0)
                return null;
            else if (columnNames.Length == 1)
            {
                object value = dr[columnName];

                if (value is DBNull)
                    return null;

                return new string[] { value.ToString() };
            }
            else
            {
                List<string> values = new List<string>();

                foreach (string colName in columnNames)
                {
                    object tempValue = dr[colName];

                    if (!(tempValue is DBNull) && tempValue != null)
                        values.Add(tempValue.ToString());
                }

                return values.ToArray();
            }
        }

        public static int GetCountFromValues(DataRow dr, string columnName, Func<string, bool> f)
        {
            string[] values = GetValuesFromDataRow(dr, columnName);

            if (values == null || values.Length == 0)
                return 0;

            return values.Count(f);
        }

        public static void ValidateRequiredColumnExists(DataColumnCollection columns, string columnName, string columnDescription)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ApplicationException("Required column '" + columnDescription + "' has not been mapped.");

            foreach (string colName in GetColumnNames(columnName))
                if (columns[colName] == null)
                    throw new ApplicationException("Required column '" + columnName + "' does not exist in Excel file.");
        }

        public static void ValidateRequiredFieldExists(DataRow dr, string columnName)
        {
            foreach (string colName in GetColumnNames(columnName))
                if (string.IsNullOrEmpty(dr[colName].ToString()))
                    throw new ApplicationException("Required field '" + colName + "' was not supplied.");
        }
    }
}
