using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.Reflection;

namespace QuantumConcepts.Common.Extensions
{
    public static class StreamExtensions
    {
        public static string GetPath(this IsolatedStorageFileStream stream)
        {
            if (stream != null)
            {
                Type type = stream.GetType();
                FieldInfo fieldInfo = type.GetField("m_FullPath", BindingFlags.Instance | BindingFlags.NonPublic);

                if (fieldInfo != null)
                    return Convert.ToString(fieldInfo.GetValue(stream));
            }

            return null;
        }
    }
}
