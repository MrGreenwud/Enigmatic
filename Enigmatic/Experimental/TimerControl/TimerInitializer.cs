using UnityEngine;

namespace Enigmatic.Experimental.TimerControl
{
    internal static class TimerInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Load()
        {
            TimerManager.Clear();

            GameObject timerUpdator = new GameObject("TimerUpdator");
            timerUpdator.AddComponent<TimerUpdator>();
        }
    }
}
