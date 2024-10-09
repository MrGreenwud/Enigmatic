using UnityEngine;

namespace Enigmatic.Experimental.KFInputSystem
{
    internal static class InputMapsProviderLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitUpdator()
        {
            GameObject updatorObject = new GameObject("InputUpdator");
            updatorObject.AddComponent<InputUpdator>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Load()
        {
            InputMapsProvider provider = InputMapResuresManager.LoadMap();
            InputManager.Load(provider);
        }
    }
}
