using System;
using System.Collections.Generic;

using UnityEngine;

namespace Enigmatic.Experimental.KFInputSystem
{
    public static class InputManager
    {
        private static InputMapsProvider sm_InputMapsProvider;
        private static List<InputMap> sm_ActiveMaps = new List<InputMap>();

        internal static void Load(InputMapsProvider provider)
        {
            sm_InputMapsProvider = provider;
        }

        internal static void Update()
        {
            foreach (InputMap map in sm_ActiveMaps)
                map.Update();
        }

        public static void EnabledMap(string name)
        {
            sm_ActiveMaps.Add(sm_InputMapsProvider.GetMap(name));
        }

        public static void DisebledMap(string name)
        {
            InputMap map = sm_InputMapsProvider.GetMap(name);

            if (sm_ActiveMaps.Contains(map) == false)
                return;

            sm_ActiveMaps.Remove(map);
        }

        public static bool GetButtonDown(string inputTag)
        {
            foreach (InputMap map in sm_ActiveMaps)
            {
                if (map.TryGetButtonDown(inputTag, out bool value))
                    return value;
            }

            return false;
        }

        public static bool GetButtonUp(string inputTag)
        {
            foreach (InputMap map in sm_ActiveMaps)
            {
                if (map.TryGetButtonUp(inputTag, out bool value))
                    return value;
            }

            return false;
        }

        public static bool GetButtonHold(string inputTag)
        {
            foreach (InputMap map in sm_ActiveMaps)
            {
                if (map.TryGetButtonHold(inputTag, out bool value))
                    return value;
            }

            return false;
        }

        public static float GetAxis(string inputTag)
        {
            foreach (InputMap map in sm_ActiveMaps)
            {
                if (map.TryGetAxis(inputTag, out float value))
                    return value;
            }

            return 0;
        }

        public static Vector2 GetAxis2D(string inputTag)
        {
            foreach (InputMap map in sm_ActiveMaps)
            {
                if (map.TryGetAxis2D(inputTag, out Vector2 value))
                    return value;
            }

            return Vector2.zero;
        }

        public static bool GetButtonDown(string mapName, string inputTag)
        {
            sm_InputMapsProvider.TryGetMap(mapName, out InputMap map);

            CheckValidInputMapName(map);

            if (map.TryGetButtonDown(inputTag, out bool value))
                return value;

            return false;
        }

        public static bool GetButtonUp(string mapName, string inputTag)
        {
            sm_InputMapsProvider.TryGetMap(mapName, out InputMap map);

            CheckValidInputMapName(map);

            if (map.TryGetButtonUp(inputTag, out bool value))
                return value;

            return false;
        }

        public static bool GetButtonHold(string mapName, string inputTag)
        {
            sm_InputMapsProvider.TryGetMap(mapName, out InputMap map);

            CheckValidInputMapName(map);

            if (map.TryGetButtonHold(inputTag, out bool value))
                return value;

            return false;
        }

        public static float GetAxis(string mapName, string inputTag)
        {
            sm_InputMapsProvider.TryGetMap(mapName, out InputMap map);

            CheckValidInputMapName(map);

            if (map.TryGetAxis(inputTag, out float value))
                return value;

            return 0;
        }

        public static Vector2 GetAxis2D(string mapName, string inputTag)
        {
            sm_InputMapsProvider.TryGetMap(mapName, out InputMap map);

            CheckValidInputMapName(map);

            if (map.TryGetAxis2D(inputTag, out Vector2 value))
                return value;

            return Vector2.zero;
        }

        public static void SubscribeButton(string inputTag, Action<bool> action)
        {
            foreach (InputMap map in sm_ActiveMaps)
            {
                if (map.TrySubscribeButton(inputTag, action))
                    return;
            }
        }

        public static void SubscribeAxis(string inputTag, Action<float> action)
        {
            foreach (InputMap map in sm_ActiveMaps)
            {
                if (map.TrySubscribeAxis(inputTag, action))
                    return;
            }
        }

        public static void SubscribeAxis2D(string inputTag, Action<Vector2> action)
        {
            foreach (InputMap map in sm_ActiveMaps)
            {
                if (map.TrySubscribeAxis2D(inputTag, action))
                    return;
            }
        }

        private static bool CheckValidInputMapName(InputMap map)
        {
            if (map == null)
                throw new Exception("");

            if (sm_ActiveMaps.Contains(map) == false)
                throw new Exception(""); //No active

            return true;
        }
    }
}
