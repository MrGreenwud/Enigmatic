using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enigmatic.Core
{
    [Serializable]
    public class EniDictionary<TKey, TValue>
    {
        [SerializeField] private List<EniKeyValuePair<TKey, TValue>> m_Element;
        
        public int Count => m_Element.Count;

        public Dictionary<TKey, TValue> ToDictionary()
        {
            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>(m_Element.Count);

            foreach(EniKeyValuePair<TKey, TValue> pair in m_Element)
                result.Add(pair.Key, pair.Value);

            return result;
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
