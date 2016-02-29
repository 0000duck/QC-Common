using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Utils
{
    public static class MathUtil
    {
        public static Random Random { get; private set; }

        static MathUtil()
        {
            MathUtil.Random = new Random();
        }

        public static void Initialize(int seed)
        {
            MathUtil.Random = new Random(seed);
        }

        public static int RandomInt(int fromInclusive, int toExclusive)
        {
            return MathUtil.Random.Next(fromInclusive, toExclusive);
        }

        public static double RandomDouble()
        {
            return MathUtil.Random.NextDouble();
        }

        public static double RandomDouble(double fromInclusive, double toExclusive)
        {
            return ((MathUtil.RandomDouble() * (toExclusive - fromInclusive)) + fromInclusive);
        }

        public static double DegreesToRadians(double degrees, int precision)
        {
            double radians = (degrees * (Math.PI / 180));

            radians = Truncate(radians, precision);

            return radians;
        }

        public static bool RandomBool(double probability = .5)
        {
            return (RandomDouble() <= probability);
        }

        public static double RadiansToDegrees(double radians, int precision)
        {
            double degrees = (radians * (180 / Math.PI));

            degrees = Truncate(degrees, precision);

            return degrees;
        }

        public static double SimplifyRadians(double radians, int precision)
        {
            double simplified = radians;
            double twoPi = (Math.PI * 2);
            double multiplier = (simplified < 0 ? -1 : 1);

            simplified = (simplified - (Math.Floor(simplified / (twoPi * multiplier)) * twoPi));
            simplified = Truncate(simplified, precision);

            if (Math.Abs(simplified) == twoPi)
                simplified = 0;

            return simplified;
        }

        public static double Truncate(double value, int precision)
        {
            double precisionPow = Math.Pow(10, precision);

            return ((double)((int)(precisionPow * value)) / precisionPow);
        }
    }
}
