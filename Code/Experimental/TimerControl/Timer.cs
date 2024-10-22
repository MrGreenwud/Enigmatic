using System;

namespace Enigmatic.Experimental.TimerControl
{
    public class Timer
    {
        public float Time { get; private set; }
        public float CurrentTime { get; private set; }

        public float Progress => CurrentTime / Time;

        public bool IsLooped { get; private set; }
        public bool IsFinished { get; private set; }
        public bool IsWaitingDestroy { get; private set; }

        public bool IsSafe { get; private set; }
        public object Owner { get; private set; }

        private bool m_Started;

        public event Action OnStart;
        public event Action OnReset;
        public event Action OnFinished;
        public event Action<Timer> OnRemove;

        public Timer(float time, bool isLooped = false)
        {
            Time = time;
            IsFinished = false;
            IsLooped = isLooped;
        }

        internal void Bind(object owner)
        {
            Owner = owner;

            if (owner != null)
                IsSafe = true;
        }

        internal void Update()
        {
            if (IsSafe && Owner == null)
                Kill();

            if (IsWaitingDestroy || IsFinished)
                return;

            if (m_Started == false)
            {
                OnStart?.Invoke();
                m_Started = true;
            }

            CurrentTime += UnityEngine.Time.deltaTime;

            if (CurrentTime >= Time)
            {
                if (IsLooped == false)
                {
                    IsFinished = true;
                    Kill();
                }

                OnFinished?.Invoke();
            }
        }

        public void Reset()
        {
            CurrentTime = 0;
            OnReset?.Invoke();
        }

        public void Kill()
        {
            IsWaitingDestroy = true;
            OnFinished = null;

            OnRemove?.Invoke(this);
        }
    }
}