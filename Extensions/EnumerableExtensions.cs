using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using QuantumConcepts.Common.Utils;

namespace QuantumConcepts.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static V TryGetValue<T, V>(this Dictionary<T, V> dictionary, T key)
        {
            if (dictionary == null || !dictionary.ContainsKey(key))
                return default(V);

            return dictionary[key];
        }

        public static V TryGetValue<T, V>(this Dictionary<T, V> dictionary, T key, V defaultValue)
        {
            if (dictionary == null || !dictionary.ContainsKey(key))
                return defaultValue;

            return dictionary[key];
        }

        public static bool TryAny<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items.IsNullOrEmpty())
                return false;

            return items.Any(predicate);
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null)
                return;

            foreach (T item in items)
                action(item);
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<int, T> action)
        {
            int index = 0;

            items.ForEach(o => action(index++, o));
        }

        public static decimal AverageOrDefault(this IEnumerable<decimal> source)
        {
            return source.AverageOrDefault(o => o, 0M);
        }

        public static decimal AverageOrDefault(this IEnumerable<decimal> source, decimal defaultValue)
        {
            return source.AverageOrDefault(o => o, defaultValue);
        }

        public static decimal AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            return source.AverageOrDefault(selector, 0M);
        }

        public static decimal AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector, decimal defaultValue)
        {
            try { return source.Average(selector); }
            catch { }

            return defaultValue;
        }
        public static int AverageOrDefault(this IEnumerable<int> source)
        {
            return source.AverageOrDefault(o => o, 0);
        }

        public static int AverageOrDefault(this IEnumerable<int> source, int defaultValue)
        {
            return source.AverageOrDefault(o => o, defaultValue);
        }

        public static int AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            return source.AverageOrDefault(selector, 0);
        }

        public static int AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector, int defaultValue)
        {
            try { return (int)source.Average(selector); }
            catch { }

            return defaultValue;
        }

        public static decimal MinOrDefault(this IEnumerable<decimal> source)
        {
            return source.MinOrDefault(o => o, 0M);
        }

        public static decimal MinOrDefault(this IEnumerable<decimal> source, decimal defaultValue)
        {
            return source.MinOrDefault(o => o, defaultValue);
        }

        public static decimal MinOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            return source.MinOrDefault(selector, 0M);
        }

        public static decimal MinOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector, decimal defaultValue)
        {
            try { return source.Min(selector); }
            catch { }

            return defaultValue;
        }

        public static decimal MaxOrDefault(this IEnumerable<decimal> source)
        {
            return source.MaxOrDefault(o => o, 0M);
        }

        public static decimal MaxOrDefault(this IEnumerable<decimal> source, decimal defaultValue)
        {
            return source.MaxOrDefault(o => o, defaultValue);
        }

        public static decimal MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            return source.MaxOrDefault(selector, 0M);
        }

        public static decimal MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector, decimal defaultValue)
        {
            try { return source.Max(selector); }
            catch { }

            return defaultValue;
        }

        public static int CountOrDefault<T>(this IEnumerable<T> source)
        {
            return source.CountOrDefault(0);
        }

        public static int CountOrDefault<T>(this IEnumerable<T> source, int defaultValue)
        {
            try { return source.Count(); }
            catch { }

            return defaultValue;
        }

        public static decimal SumOrDefault(this IEnumerable<decimal> source)
        {
            return source.SumOrDefault(o => o, 0M);
        }

        public static decimal SumOrDefault(this IEnumerable<decimal> source, decimal defaultValue)
        {
            return source.SumOrDefault(o => o, defaultValue);
        }

        public static int SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            return source.SumOrDefault(selector, 0);
        }

        public static decimal SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            return source.SumOrDefault(selector, 0M);
        }

        public static int SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector, int defaultValue)
        {
            try { return source.Sum(selector); }
            catch { }

            return defaultValue;
        }

        public static decimal SumOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector, decimal defaultValue)
        {
            try { return source.Sum(selector); }
            catch { }

            return defaultValue;
        }

        public static bool AnyOrDefault<T>(this IEnumerable<T> items, Func<T, bool> selector)
        {
            if (items == null)
                return false;

            return items.Any(selector);
        }

        public static bool IsNullOrEmpty(this IEnumerable items)
        {
            return (items == null || !items.GetEnumerator().MoveNext());
        }

        /// <summary>Extracts all element from the IEnumerable and returns them in a List.</summary>
        /// <typeparam name="T">The type of the IEnumerable as well as the List that is returned.</typeparam>
        /// <param name="items">The IEnumerable to convert to a List.</param>
        /// <returns>A List containing all elements in the IEnumerable, or an empty List..</returns>
        public static List<T> ToListOrEmpty<T>(this IEnumerable<T> items)
        {
            if (items == null)
                return new List<T>();

            return items.ToList();
        }

        public static T Last<T>(this IEnumerable items)
        {
            return items.Cast<T>().Last();
        }

        public static T Peek<T>(this List<T> items, int index)
        {
            if (index < 0 || index > (items.Count - 1))
                return default(T);

            return items[index];
        }

        public static bool All<T>(this IEnumerable<T> items, Func<int, T, bool> action)
        {
            int index = 0;

            return items.All(o => action(index++, o));
        }

        public static IEnumerable<T> NonNull<T>(this IEnumerable<T> items)
            where T : class
        {
            return items.Where(o => o != null);
        }

        public static T TryGetEntry<T>(this IList<T> list, int index)
        {
            if (list.IsNullOrEmpty() || index < 0 || index > (list.Count - 1))
                return default(T);

            return list[index];
        }

        public static bool EqualsAny<T>(this T item, IEnumerable<T> values)
        {
            return values.Any(o => object.Equals(item, o));
        }

        public static IEnumerable<T1> OfType<T, T1, T2>(this IEnumerable<T> items)
        {
            return items.OfType<T2>().OfType<T1>();
        }

        public static object Random(this IEnumerable items)
        {
            return items.Cast<object>().Random();
        }

        public static T Random<T>(this IEnumerable<T> items, Random random = null)
        {
            int count = items.Count();

            if (count == 0)
                return default(T);
            else
            {
                int index = (random ?? MathUtil.Random).Next(0, items.Count());

                return items.Skip(index).Take(1).Single();
            }
        }

        public static bool IsInBounds(this IList items, int index)
        {
            return (index >= 0 && index < items.Count);
        }

        public static void Remove<T>(this List<T> items, Predicate<T> predicate)
        {
            for (int i = (items.Count - 1); i >= 0; i--)
            {
                T item = items[i];

                if (predicate(item))
                    items.Remove(item);
            }
        }
    }
}
