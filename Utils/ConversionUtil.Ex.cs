using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace QuantumConcepts.Common.Utils
{
    public partial class ConversionUtil
    {
        public static Color HexStringToColor(string hex)
        {
            hex = hex.Replace("#", "");

            if (hex.Length != 6)
                throw new Exception(hex + " is not a valid 6-place hexadecimal color code.");

            string r, g, b;

            r = hex.Substring(0, 2);
            g = hex.Substring(2, 2);
            b = hex.Substring(4, 2);

            return Color.FromArgb(HexStringToBase10Int(r), HexStringToBase10Int(g), HexStringToBase10Int(b));
        }

        public static string ColorToHexString(Color color)
        {
            if (color == Color.Transparent)
                return null;

            return (Base10ToHexString(Convert.ToInt16(color.R)) + Base10ToHexString(Convert.ToInt16(color.G)) + Base10ToHexString(Convert.ToInt16(color.B)));
        }
    }
}
