using System.Collections.Generic;

namespace Enigmatic.Core
{
    public static class ListExtansions
    {
        public static List<T> Clone<T>(this List<T> list)
        {
            List<T> tempList = new List<T>(list.Count);

            tempList.AddRange(list);

            return tempList;
        }

        public static List<T> Combine<T>(this List<T> listA, List<T> listB)
        {
            List<T> result = new List<T>(listA.Count + listB.Count);

            result.AddRange(listA);
            result.AddRange(listB);

            return result;
        }

        public static List<T> CombineNoSeem<T>(this List<T> list, T[] array)
        {
            List<T> result = new List<T>(list.Count + array.Length);

            result.AddRange(list);

            foreach (T item in array)
            {
                if (result.Contains(item))
                    continue;

                result.Add(item);
            }

            return result;
        }
    }
}
