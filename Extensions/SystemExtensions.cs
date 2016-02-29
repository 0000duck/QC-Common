using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Extensions
{
    public static class SystemExtensions
    {
        public static bool NextBool(this Random random)
        {
            return (random.Next(0, 2) == 0);
        }

        public static bool IsAbovePercentage(this Random random, double percentage)
        {
            return (percentage < random.NextDouble());
        }

        public static char NextAlpha(this Random random)
        {
            return (char)random.Next((int)'A', ((int)'Z') + 1);
        }
    }
}