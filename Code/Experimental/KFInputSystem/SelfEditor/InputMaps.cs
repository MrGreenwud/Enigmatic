using System;
using System.Collections.Generic;

using Enigmatic.Experimental.ENIX;

namespace Enigmatic.Experimental.KFInputSystem.Editor
{
    [SerializebleObject]
    internal class InputMaps
    {
        [SerializebleProperty] private List<InputMapSettings> m_InputMaps = new List<InputMapSettings>();

        public int Count => m_InputMaps.Count;

        public void Add(InputMapSettings inputMap) => m_InputMaps.Add(inputMap);
        public void Remove(InputMapSettings inputMap) => m_InputMaps.Remove(inputMap);

        public void ForEach(Action<InputMapSettings> action)
        {
            foreach (InputMapSettings mapSettings in m_InputMaps)
                action?.Invoke(mapSettings);
        }

        public int IndexOf(InputMapSettings inputMap) => m_InputMaps.IndexOf(inputMap);

        public InputMapSettings Get(int index) => m_InputMaps[index];
    }
}
