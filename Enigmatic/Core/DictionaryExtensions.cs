using System.Collections.Generic;

namespace Enigmatic.Core
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>(dictionary.Count);

            foreach (TKey key in dictionary.Keys)
                result.Add(key, dictionary[key]);

            return result;
        }
    }
}
