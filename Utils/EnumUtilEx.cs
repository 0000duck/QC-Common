using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using QuantumConcepts.Common.Utils.DescriptiveEnum;

namespace QuantumConcepts.Common.Utils
{
    public static partial class EnumUtil
    {
        public static object DatabaseValueToEnumValue(Type type, string databaseValue)
        {
            if (string.IsNullOrEmpty(databaseValue))
                return null;

            if (!type.IsEnum)
                throw new ApplicationException("Type parameter must be an enum.");

            foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
            {
                foreach (System.Attribute attribute in fieldInfo.GetCustomAttributes(typeof(DatabaseValueAttribute), false))
                {
                    if (attribute.ToString().Equals(databaseValue))
                        return Enum.Parse(type, fieldInfo.Name);
                }
            }

            throw new ApplicationException("Specified database value of '" + databaseValue + "' does not exist in enum of type '" + type.Name + "'.");
        }

        public static IEnumerable<EnumValue<T>> GetEnumValues<T>()
        {
            Type type = typeof(T);

            if (!type.IsEnum)
                throw new ApplicationException("Type parameter must be an enum.");

            return GetEnumValues<T>(Enum.GetValues(type).OfType<T>());
        }

        public static IEnumerable<EnumValue<T>> GetEnumValues<T>(IEnumerable<T> items)
        {
            Type type = typeof(T);

            if (!type.IsEnum)
                throw new ApplicationException("Type parameter must be an enum.");

            foreach (object item in items)
                yield return new EnumValue<T>((T)item);
        }
    }

    public class EnumValue<T> : IComparable<EnumValue<T>>, IEquatable<EnumValue<T>>, IEquatable<T>
    {
        private T _value;
        private string _valueString;
        private string _description;

        public T Value { get { return _value; } }
        public string ValueString { get { return _valueString; } }
        public string Description { get { return _description; } }

        public EnumValue(T value)
        {
            if (value == null)
                throw new ApplicationException("Null value may not be passed into this constructor.");

            if (!value.GetType().IsEnum)
                throw new ApplicationException("Only enum types may be passed into this constructor.");

            _value = value;
            _valueString = EnumUtil.GetName<T>(value);
            _description = EnumUtil.GetDescription<T>(value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public int CompareTo(EnumValue<T> other)
        {
            return string.Compare(this.ValueString, other.ValueString);
        }

        public bool Equals(EnumValue<T> other)
        {
            return string.Equals(this.Value.ToString(), this.ToString());
        }

        public bool Equals(T other)
        {
            return string.Equals(this.Value.ToString(), this.ToString());
        }

        public override bool Equals(object obj)
        {
            return string.Equals(this.Value.ToString(), obj.ToString());
        }

        public override string ToString()
        {
            return this.ValueString;
        }
    }
}
