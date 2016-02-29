using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using QuantumConcepts.Common.Utils.DescriptiveEnum;
using System.Text.RegularExpressions;

namespace QuantumConcepts.Common.Utils
{
    public static partial class EnumUtil
    {
        public static string GetName(this Enum value)
        {
            return GetEnumName(value);
        }

        public static string GetName<T>(object value)
        {
            return GetEnumName(value);
        }

        public static string GetDescription(this Enum value)
        {
            return GetEnumDescription(value);
        }

        public static string GetDescription<T>(object value)
        {
            return GetEnumDescription(value);
        }

        public static string EnumValueToDatabaseValue(object value)
        {
            if (value == null)
                return null;

            Type type = value.GetType();

            if (!type.IsEnum)
                throw new ApplicationException("Value parameter must be an enum.");

            FieldInfo fieldInfo = type.GetField(value.ToString());
            object[] attributes = fieldInfo.GetCustomAttributes(typeof(DatabaseValueAttribute), false);

            if (attributes == null || attributes.Length == 0)
                throw new ApplicationException("No attributes exist in enum of type '" + type.Name + "'.");
            else if (attributes.Length > 1)
                throw new ApplicationException("Too many DatabaseValue attributes exist in enum of type '" + type.Name + "'.");

            return attributes[0].ToString();
        }

        public static string GetEnumName(object value)
        {
            if (value == null)
                return null;

            Type type = value.GetType();

            if (!type.IsEnum)
                throw new ApplicationException("Value parameter must be an enum.");

            FieldInfo fieldInfo = type.GetField(value.ToString());
            object[] nameAttributes = fieldInfo.GetCustomAttributes(typeof(NameAttribute), false);

            if (nameAttributes == null || nameAttributes.Length == 0)
            {
                object[] enforcementAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptiveEnumEnforcementAttribute), false);

                if (enforcementAttributes != null && enforcementAttributes.Length == 1)
                {
                    DescriptiveEnumEnforcementAttribute enforcementAttribute = (DescriptiveEnumEnforcementAttribute)enforcementAttributes[0];

                    if (enforcementAttribute.EnforcementType == DescriptiveEnumEnforcementAttribute.EnforcementTypeEnum.ThrowException)
                        throw new ApplicationException("No Name attributes exist in enforced enum of type '" + type.Name + "', value '" + value.ToString() + "'.");

                    return EnumValueToDisplayString(value.ToString());
                }
                else
                    return EnumValueToDisplayString(value.ToString());
            }
            else if (nameAttributes.Length > 1)
                throw new ApplicationException("Too many Name attributes exist in enum of type '" + type.Name + "', value '" + value.ToString() + "'.");

            return nameAttributes[0].ToString();
        }

        public static string GetEnumDescription(object value)
        {
            if (value == null)
                return null;

            Type type = value.GetType();

            if (!type.IsEnum)
                throw new ApplicationException("Value parameter must be an enum.");

            FieldInfo fieldInfo = type.GetField(value.ToString());
            object[] descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descriptionAttributes == null || descriptionAttributes.Length == 0)
            {
                object[] enforcementAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptiveEnumEnforcementAttribute), false);

                if (enforcementAttributes != null && enforcementAttributes.Length == 1)
                {
                    DescriptiveEnumEnforcementAttribute enforcementAttribute = (DescriptiveEnumEnforcementAttribute)enforcementAttributes[0];

                    if (enforcementAttribute.EnforcementType == DescriptiveEnumEnforcementAttribute.EnforcementTypeEnum.ThrowException)
                        throw new ApplicationException("No Description attributes exist in enforced enum of type '" + type.Name + "', value '" + value.ToString() + "'.");

                    return GetEnumName(value);
                }
                else
                    return GetEnumName(value);
            }
            else if (descriptionAttributes.Length > 1)
                throw new ApplicationException("Too many Description attributes exist in enum of type '" + type.Name + "', value '" + value.ToString() + "'.");

            return descriptionAttributes[0].ToString();
        }

        public static string EnumValueToDisplayString(string enumValue)
        {
            return Regex.Replace(enumValue, "(?-i)([A-Z])", " $1").Trim();
        }
    }
}
