using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace TimeManagment
{
    public class TimeManager : MonoBehaviour
    {
        private List<Timer> m_Timers = new List<Timer>();
        private Queue<TimerBase> m_DestroyedTimers = new Queue<TimerBase>();

        private static ProfilerMarker s_TimerUpdate = new ProfilerMarker(ProfilerCategory.Scripts, "Timer Update");
        private static ProfilerMarker s_TimerDestroy = new ProfilerMarker(ProfilerCategory.Scripts, "Timer Destroy");

        public TimerBase CreatTimer(float time, bool isLooped = false)
        {
            Timer timer = new Timer(time, isLooped);
            m_Timers.Add(timer);
            timer.AddLisenerDestroyed(DestroyTimer);
            return timer;
        }

        private void Update()
        {
            s_TimerUpdate.Begin();

            foreach (Timer timer in m_Timers)
                timer.Tick();

            s_TimerUpdate.End();
        }

        private void LateUpdate()
        {
            s_TimerDestroy.Begin();

            while (m_DestroyedTimers.Count > 0)
            {
                Timer timer = m_DestroyedTimers.Dequeue() as Timer;
                m_Timers.Remove(timer);
            }

            s_TimerDestroy.End();
        }

        public TimerBase GetTimerBySender(object sender)
        {
            foreach(Timer timer in m_Timers)
                if(timer.Sender == sender)
                    return timer;

            return null;
        }

        public TimerBase[] GetTimersBySender(object sender)
        {
            List<Timer> timers = new List<Timer>();

            foreach (Timer timer in m_Timers)
                if (timer.Sender == sender)
                    timers.Add(timer);

            return timers.ToArray();
        }

        private void DestroyTimer(TimerBase timer)
        {
            m_DestroyedTimers.Enqueue(timer);
            timer.RemoveLisenerDestroyed(DestroyTimer);
        }
    }

    public class TimerBase
    {
        protected float m_time { get; private set; }
        protected float m_currentTime { get; private set; }

        private event Action OnStarted;
        private event Action OnFineshed;
        private event Action<TimerBase> OnDestroyed;

        public object Sender { get; private set; }

        protected bool m_isLooped { get; private set; }
        protected bool m_isFinesh { get; private set; }
        protected bool m_isWaitDestroy { get; private set; }

        protected bool m_isPouse { get; private set; }
        
        protected bool m_isSafely { get; private set; }

        public TimerBase(float time, bool isLooped = false)
        {
            this.m_time = time;
            m_currentTime = time;
            this.m_isLooped = isLooped;
        }

        public TimerBase Safely(object sender)
        {
            m_isSafely = true;
            Sender = sender;

            return this;
        }

        #region Follow event methods

        public TimerBase AddLisenerStarted(Action action)
        {
            OnStarted += action;
            return this;
        }

        public TimerBase AddLisenerFineshed(Action action)
        {
            OnFineshed += action;
            return this;
        }

        public TimerBase AddLisenerDestroyed(Action<TimerBase> action)
        {
            OnDestroyed += action;
            return this;
        }

        #endregion

        #region Unfollow event methods
        
        public TimerBase RemoveLisenerStarted(Action action)
        {
            OnStarted += action;
            return this;
        }

        public TimerBase RemoveLisenerFineshed(Action action)
        {
            OnFineshed += action;
            return this;
        }

        public TimerBase RemoveLisenerDestroyed(Action<TimerBase> action)
        {
            OnDestroyed += action;
            return this;
        }

        #endregion

        public void Reset()
        {
            m_isFinesh = false;
            Started();
        }

        public void Pouse() => m_isPouse = true;

        public void Continue() => m_isPouse = false;

        public void Kill() => Destroyed();

        protected void Started() => OnStarted?.Invoke();
        
        protected void Fineshed() 
        {
            OnFineshed?.Invoke();
            m_isFinesh = true;
        }

        protected void Destroyed()
        {
            m_isWaitDestroy = true;
            OnDestroyed?.Invoke(this);
        }

        protected void Next() => m_currentTime -= Time.deltaTime;
    }

    internal class Timer : TimerBase
    {
        public Timer(float time, bool isLooped = false) : base(time, isLooped) { }

        public void Tick()
        {
            if (m_isFinesh == true || m_isWaitDestroy == true)
                return;

            if (m_isSafely == true)
            {
                if (Sender.ToString() == "null")
                {
                    Kill();
                    return;
                }
            }

            if (m_isPouse == true)
                return;

            Next();

            if(m_currentTime <= 0)
            {
                Fineshed();

                if (m_isLooped == true)
                    Reset();
                else
                    Destroyed();
            }
        }
    }
}
