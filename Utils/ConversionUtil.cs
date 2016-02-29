using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace QuantumConcepts.Common.Utils
{
    public partial class ConversionUtil
    {
        public static int HexStringToBase10Int(string hex)
        {
            int base10value = 0;

            try { base10value = Convert.ToInt32(hex, 16); }
            catch { }

            return base10value;
        }

        public static string Base10ToHexString(int base10)
        {
            return base10.ToString("X");
        }
    }
}
