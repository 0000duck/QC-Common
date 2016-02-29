using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace QuantumConcepts.Common.Utils
{
    public static class ProcessUtil
    {
        public static void LaunchUrl(string url)
        {
            Process.Start(url, null);
        }
    }
}
