using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageResizer
{
    public static class Enumerable2
    {
        public static bool TryGetItem<T>(this T[] source, int index, out T item)
        {
            if (source.Length <= index)
            {
                item = default(T);
                return false;
            }

            item = source[index];
            return true;
        }

        public static int? FirstIndex<T>(this T[] source, Func<T, bool> predicate)
        {
            for (var i = 0; i < source.Length; i++)
                if (predicate(source[i]))
                    return i;

            return null;
        }
    }
}
