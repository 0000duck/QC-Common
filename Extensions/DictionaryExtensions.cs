using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Extensions
{
    /// <summary>Extends the functionality of the generic Dictionary type.</summary>
    public static class DictionaryExtensions
    {
        /// <summary>Interrogates the provided dictionary for the specified key.</summary>
        /// <typeparam name="K">The type of the key.</typeparam>
        /// <typeparam name="V">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to interrogate.</param>
        /// <param name="key">The key to try to find in the dictionary.</param>
        /// <returns>The value of the key in the dictionary, or null if it does not exist or the dictionary is null.</returns>
        public static V ValueOrDefault<K, V>(this IDictionary<K, V> dictionary, K key)
        {
            if (dictionary != null && dictionary.ContainsKey(key))
                return dictionary[key];

            return default(V);
        }

        /// <summary>Interrogates the provided dictionary for the specified key.</summary>
        /// <typeparam name="K">The type of the key.</typeparam>
        /// <typeparam name="V">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to interrogate.</param>
        /// <param name="key">The key to try to find in the dictionary.</param>
        /// <returns>The value of the key in the dictionary, or null if it does not exist or the dictionary is null.</returns>
        public static object ValueOrDefault(this IDictionary dictionary, object key)
        {
            if (dictionary != null && dictionary.Contains(key))
                return dictionary[key];

            return null;
        }

        /// <summary>Interrogates the provided dictionary for the specified key.</summary>
        /// <typeparam name="K">The type of the key.</typeparam>
        /// <typeparam name="V">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to interrogate.</param>
        /// <param name="key">The key to try to find in the dictionary.</param>
        /// <returns>The value of the key in the dictionary, or null if it does not exist or the dictionary is null.</returns>
        public static V ValueOr<K, V>(this IDictionary<K, V> dictionary, K key, V defaultValue)
            where V : class
        {
            return (dictionary.ValueOrDefault(key) ?? defaultValue);
        }

        /// <summary>Interrogates the provided dictionary for the specified key.</summary>
        /// <typeparam name="K">The type of the key.</typeparam>
        /// <typeparam name="V">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to interrogate.</param>
        /// <param name="key">The key to try to find in the dictionary.</param>
        /// <returns>The value of the key in the dictionary, or null if it does not exist or the dictionary is null.</returns>
        public static object ValueOr(this IDictionary dictionary, object key, object defaultValue)
        {
            return (dictionary.ValueOrDefault(key) ?? defaultValue);
        }

        public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> pairs)
        {
            return pairs.ToDictionary(o => o.Key, o => o.Value);
        }

        public static void Set<K, V>(this Dictionary<K, V> dictionary, K key, V value)
        {
            if (!dictionary.ContainsKey(key))
                dictionary.Add(key, value);
            else
                dictionary[key] = value;
        }

        public static void Set<K1, K2, V>(this Dictionary<K1, Dictionary<K2, V>> dictionary, K1 key1, K2 key2, V value)
        {
            if (!dictionary.ContainsKey(key1))
                dictionary.Add(key1, new Dictionary<K2, V>() { { key2, value } });
            else if (!dictionary[key1].ContainsKey(key2))
                dictionary[key1].Add(key2, value);
            else
                dictionary.Add(key1, new Dictionary<K2, V>() { { key2, value } });
        }
    }
}