using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Common.Utils
{
    public static class SampleDataUtil
    {
        public static int Seed { get; private set; }
        public static Random Random { get { return MathUtil.Random; } }

        public static void Initialize(int seed)
        {
            SampleDataUtil.Seed = seed;
            MathUtil.Initialize(seed);
        }

        public static DateTime GetRandomDate()
        {
            return GetRandomDate(null, null);
        }

        public static DateTime GetRandomDate(DateTime? minDate, DateTime? maxDate)
        {
            DateTime value;

            if (!minDate.HasValue)
                minDate = new DateTime(1925, 1, 1);

            if (!maxDate.HasValue)
                maxDate = new DateTime(2025, 12, 31);

            value = minDate.Value;
            value = value.AddDays(SampleDataUtil.Random.Next(0, (int)(maxDate.Value - minDate.Value).TotalDays + 1));

            return value;
        }

        public static long GetRandomNumber(int length)
        {
            return long.Parse(GetRandomNumberAsString(length));
        }

        public static string GetRandomNumberAsString(int length)
        {
            StringBuilder number = new StringBuilder(length);

            for (int i = 0; i < length; i++)
                number.Append(SampleDataUtil.Random.Next(0, 10));

            return number.ToString();
        }

        public static T GetRandomEnum<T>()
        {
            Type type = typeof(T);
            Array values = null;

            if (!type.IsEnum)
                throw new ArgumentException("Must be an enum.", "T");

            values = Enum.GetValues(type);

            return (T)values.GetValue(SampleDataUtil.Random.Next(0, values.Length));
        }

        public static string GetRandomLineFromFile(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                int lineCount = int.Parse(reader.ReadLine());
                int skip = SampleDataUtil.Random.Next(0, lineCount);

                for (int l = 0; l < skip; l++)
                    reader.ReadLine();

                return reader.ReadLine();
            }
        }

        public static T GetRandomObject<T>(IList<T> objects)
        {
            if (objects.IsNullOrEmpty())
                return default(T);

            return objects[SampleDataUtil.Random.Next(0, objects.Count)];
        }
    }
}
