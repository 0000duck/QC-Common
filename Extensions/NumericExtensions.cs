using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QuantumConcepts.Common.Extensions
{
    public static class NumericExtensions
    {
        public static string ToCurrencyString(this double value)
        {
            return ToCurrencyString((decimal)value);
        }

        public static string ToCurrencyString(this decimal value)
        {
            return value.ToString("C");
        }

        public static string ToDisplayString(this int value)
        {
            return value.ToString("N0");
        }

        public static string ToDisplayString(this long value)
        {
            return value.ToString("N0");
        }

        public static string ToDisplayString(this decimal value)
        {
            return value.ToString("N2");
        }

        public static string ToDisplayString(this decimal value, int decimalPlaces)
        {
            return value.ToString("N{0}".FormatString(decimalPlaces));
        }

        public static bool IsInt(this string valueString)
        {
            int value;

            return int.TryParse(valueString, out value);
        }

        public static int ToInt(this string valueString)
        {
            if (!valueString.IsInt())
                throw new FormatException("The value \"{0}\" is not a valid string.".FormatString(valueString));

            return int.Parse(valueString);
        }

        public static int? ToNullableInt(this string valueString)
        {
            return (valueString.IsInt() ? (int?)valueString.ToInt() : null);
        }

        public static bool IsBool(this string valueString)
        {
            bool value;

            return bool.TryParse(valueString, out value);
        }

        public static bool? ToNullableBool(this string valueString)
        {
            return (valueString.IsBool() ? (bool?)valueString.ToBool() : null);
        }

        public static bool ToBool(this string valueString)
        {
            if (!valueString.IsBool())
                throw new FormatException("The value \"{0}\" is not a valid bool.".FormatString(valueString));

            return bool.Parse(valueString);
        }

        public static string ToFileSize(this int sizeInBytes)
        {
            return ToFileSize((long)sizeInBytes);
        }

        public static string ToFileSize(this long sizeInBytes)
        {
            decimal kb = (sizeInBytes / 1024);
            decimal mb = (kb / 1024);
            decimal gb = (mb / 1024);
            decimal value = (gb >= 1 ? gb : (mb >= 1 ? mb : (kb >= 1 ? kb : sizeInBytes)));
            string unit = (gb >= 1 ? "GB" : (mb >= 1 ? "MB" : (kb >= 1 ? "KB" : "B")));
            string format = (gb >= 1 ? "N2" : (mb >= 1 ? "N2" : "N0"));

            return "{0} {1}".FormatString(value.ToString(format), unit);
        }

        public static decimal ParsePercentage(this string value)
        {
            decimal percentageValue = 0M;

            if (!string.IsNullOrEmpty(value))
            {
                Match match = Regex.Match(value, @"(?<Value>\d+)");

                if (match.Success)
                    percentageValue = (decimal.Parse(match.Groups["Value"].Value) / 100M);
            }

            return percentageValue;
        }

        public static float ToRadians(this float value)
        {
            return (float)((double)value).ToRadians();
        }

        public static decimal ToRadians(this decimal value)
        {
            return (decimal)((double)value).ToRadians();
        }

        public static double ToRadians(this double value)
        {
            return (value * (Math.PI / 180d));
        }
    }
}
