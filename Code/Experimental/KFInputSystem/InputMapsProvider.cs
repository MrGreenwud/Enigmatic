using System;
using System.Collections.Generic;

using Enigmatic.Experimental.ENIX;

namespace Enigmatic.Experimental.KFInputSystem
{
    [SerializebleObject]
    internal class InputMapsProvider
    {
        [SerializebleProperty] private Dictionary<string, InputMap> m_Maps;

        public InputMapsProvider()
        {
            m_Maps = new Dictionary<string, InputMap>();
        }

        public void Update()
        {
            foreach (InputMap map in m_Maps.Values)
                map.Update();
        }

        public void AddMap(string name)
        {
            if (m_Maps.ContainsKey(name))
                throw new Exception(" ");
        }

        public void AddMap(string name, InputMap map)
        {
            if (m_Maps.ContainsKey(name))
                throw new Exception(" ");

            m_Maps.Add(name, map);
        }

        public InputMap GetMap(string name)
        {
            if (m_Maps.ContainsKey(name) == false)
                throw new Exception();

            return m_Maps[name];
        }

        public bool TryGetMap(string name, out InputMap map)
        {
            map = null;

            if (m_Maps.ContainsKey(name) == false)
                return false;

            map = m_Maps[name];
            return true;
        }
    }
}
