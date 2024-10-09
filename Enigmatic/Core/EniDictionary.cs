using System;
using System.Collections.Generic;

using UnityEngine;

namespace Enigmatic.Core
{
    [Serializable]
    public class EniDictionary<TKey, TValue>
    {
        [SerializeField] private EniKeyValuePair<TKey, TValue>[] m_Elements;

        public Dictionary<TKey, TValue> ToDictionary()
        {
            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>(m_Elements.Length);

            foreach(EniKeyValuePair<TKey, TValue> pair in m_Elements)
                result.Add(pair.Key, pair.Value);

            return result;
        }

        public void Clear()
        {
            m_Elements = new EniKeyValuePair<TKey, TValue>[0];
        }
    }

    [Serializable]
    public struct EniKeyValuePair<TKey, TValue>
    {
        [SerializeField] private TKey m_Key;
        [SerializeField] private TValue m_Value;

        public TKey Key => m_Key;
        public TValue Value => m_Value;
    }
}
