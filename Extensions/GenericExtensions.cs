using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Extensions
{
    public static class GenericExtensions
    {
        /// <summary>If obj is null, default(R) is returned, otherwise the result of the selector is returned.</summary>
        /// <typeparam name="T">The type to act upon.</typeparam>
        /// <typeparam name="R">The return type.</typeparam>
        /// <param name="obj">The object from which to attempt to select the value.</param>
        /// <param name="selector">The selector to use upon the object in order to select the value.</param>
        /// <returns>Either default(R) or selector(R).</returns>
        public static R ValueOrDefault<T, R>(this T obj, Func<T, R> selector)
            where T : class
        {
            if (obj == null)
                return default(R);

            return selector(obj);
        }

        /// <summary>If obj is null, default(R) is returned, otherwise the result of the selector is returned.</summary>
        /// <typeparam name="T">The type to act upon.</typeparam>
        /// <typeparam name="R">The return type.</typeparam>
        /// <param name="obj">The object from which to attempt to select the value.</param>
        /// <param name="selector">The selector to use upon the object in order to select the value.</param>
        /// <returns>Either default(R) or selector(R).</returns>
        public static R ValueOrDefault<T, R>(this Nullable<T> obj, Func<T, R> selector, R defaultValue)
            where T : struct
        {
            if (!obj.HasValue)
                return defaultValue;

            return selector(obj.Value);
        }

        /// <summary>If obj is null, defaultValue is returned, otherwise obj is returned.</summary>
        /// <typeparam name="R">The return type.</typeparam>
        /// <param name="obj">The object from which to attempt to select the value.</param>
        /// <param name="defaultValue">The value to return if obj is null.</param>
        /// <returns>Either defaultValue or obj.</returns>
        public static R ValueOr<R>(this R obj, R defaultValue)
            where R : class
        {
            return (obj ?? defaultValue);
        }

        /// <summary>If obj is null, defaultValue is returned, otherwise obj is returned.</summary>
        /// <typeparam name="R">The return type.</typeparam>
        /// <param name="obj">The object from which to attempt to select the value.</param>
        /// <param name="defaultValue">The value to return if obj is null.</param>
        /// <returns>Either defaultValue or obj.</returns>
        public static R ValueOr<T, R>(this T obj, Func<T, R> selector, R defaultValue)
            where T : class
        {
            if (obj == null)
                return defaultValue;

            return selector(obj);
        }

        /// <summary>Accepts a list of selectors to be applied to the given instance; the first non-null result is returned.</summary>
        /// <typeparam name="T">The type of object upon which the selectors are based.</typeparam>
        /// <typeparam name="R">The return type.</typeparam>
        /// <param name="instance">The instance upon which to apply the selectors and extract results.</param>
        /// <param name="selectors">The selectors to apply to the instance, in turn.</param>
        /// <returns>The first non-null result as each selector is applied to the instance.</returns>
        public static R Coalesce<T, R>(this T instance, params Func<T, R>[] selectors)
            where R : class
        {
            if (selectors == null)
                return null;

            foreach (var selector in selectors)
            {
                R result = selector(instance);

                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>Determines if a nullable bool is true.</summary>
        public static bool IsTrue(this bool? value)
        {
            return (value.HasValue && value.Value);
        }

        /// <summary>Determines if a nullable bool is false.</summary>
        public static bool IsFalse(this bool? value)
        {
            return !value.IsTrue();
        }

        /// <summary>Attempts to cast the provided object to the type specified by the generic argument.</summary>
        /// <remarks>An exception will be thrown if the cast is not possible.</remarks>
        public static T As<T>(this object toCast) where T : class {
            return (T)toCast;
        }

        /// <summary>Attempts to cast the provided object to the type specified by the generic argument.</summary>
        /// <remarks>Returns null if the cast is not possible.</remarks>
        public static T TryCast<T>(this object toCast) where T : class {
            return (toCast as T);
        }
    }
}
