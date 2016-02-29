using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using QuantumConcepts.Common.Utils;

namespace QuantumConcepts.Common.Net.Rest.Transmission
{
    public partial class ApiParameter
    {
        public static ApiParameter Create(string key, Color? value)
        {
            return new ApiParameter(key, (value.HasValue ? ConversionUtil.ColorToHexString(value.Value) : null));
        }
    }
}
