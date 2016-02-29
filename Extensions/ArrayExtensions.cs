using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Extensions
{
    public static class ArrayExtensions
    {
        public static bool IsInBounds(this Array array, int index)
        {
            return array.IsInBounds(0, index);
        }

        public static bool IsInBounds(this Array array, int dimension, int index)
        {
            return (index >= 0 && index <= array.GetUpperBound(dimension));
        }

        public static bool IsInBounds(this Array array, params int[] indices)
        {
            for (int i = 0; i < indices.Length; i++)
                if (!array.IsInBounds(i, indices[i]))
                    return false;

            return true;
        }

        public static object TryGet(this Array array, params int[] indices)
        {
            if (!array.IsInBounds(indices))
                return null;

            return array.GetValue(indices);
        }

        public static object TryGet(this Array array, object defaultvalue, params int[] indices)
        {
            if (!array.IsInBounds(indices))
                return defaultvalue;

            return array.GetValue(indices);
        }
    }
}
