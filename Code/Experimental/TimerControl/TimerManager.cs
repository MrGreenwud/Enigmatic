using System;
using System.Collections.Generic;

using UnityEngine;

namespace Enigmatic.Experimental.TimerControl
{
    public static class TimerManager
    {
        private static List<Timer> m_ActiveTimers = new List<Timer>();
        private static Queue<Timer> m_DestroyQueue = new Queue<Timer>();

        public static Timer CreateTimer(float time, bool isLooped = false, object owner = null)
        {
            Timer timer = new Timer(time, isLooped);
            timer.Bind(owner);
            timer.OnRemove += RemoveTimer;
            m_ActiveTimers.Add(timer);

            return timer;
        }

        internal static void Update()
        {
            foreach(Timer timer in m_ActiveTimers)
                timer.Update();
        }

        internal static void LateUpdate()
        {
            while(m_DestroyQueue.Count > 0)
            {
                Timer timer = m_DestroyQueue.Dequeue();
                m_ActiveTimers.Remove(timer);
            }
        }

        internal static void Clear()
        {
            m_ActiveTimers.Clear();
            m_DestroyQueue.Clear();
        }

        private static void RemoveTimer(Timer timer)
        {
            m_DestroyQueue.Enqueue(timer);
        }
    }
}