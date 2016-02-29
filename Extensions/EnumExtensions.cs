using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumConcepts.Common.Utils;

namespace QuantumConcepts.Common.Extensions
{
    public static class EnumExtensions
    {
        public static bool TryParse<T>(string value, out T enumValue)
        {
            enumValue = default(T);

            if (string.IsNullOrEmpty(value))
                return false;

            try
            {
                enumValue = (T)Enum.Parse(typeof(T), value);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
