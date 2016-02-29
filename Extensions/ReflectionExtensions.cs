using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace QuantumConcepts.Common.Extensions
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<T> GetAttributes<T>(this Type type)
            where T : Attribute
        {
            object[] attributes = type.GetCustomAttributes(typeof(T), true);

            if (attributes.IsNullOrEmpty())
                return null;

            return attributes.Cast<T>();
        }

        public static T GetAttribute<T>(this Type type)
            where T : Attribute
        {
            IEnumerable<T> attributes = type.GetAttributes<T>();

            if (attributes.IsNullOrEmpty())
                return null;

            return attributes.First();
        }

        public static IEnumerable<T> GetAttributes<T>(this PropertyInfo propertyInfo)
            where T : Attribute
        {
            object[] attributes = propertyInfo.GetCustomAttributes(typeof(T), true);

            if (attributes.IsNullOrEmpty())
                return null;

            return attributes.Cast<T>();
        }

        public static T GetAttribute<T>(this PropertyInfo propertyInfo)
            where T : Attribute
        {
            IEnumerable<T> attributes = propertyInfo.GetAttributes<T>();

            if (attributes.IsNullOrEmpty())
                return null;

            return attributes.First();
        }
    }
}
