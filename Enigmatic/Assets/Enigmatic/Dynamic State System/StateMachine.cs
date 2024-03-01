using System.Collections.Generic;
using UnityEngine;

namespace Enigmatic.DynamicStateSystem
{
    public class StateMachine : MonoBehaviour
    {
        private List<State> m_States = new List<State>();

        public State CurrentState { get; private set; }

        public void SwichState(State newState)
        {
            #if UNITY_EDITOR
            {
                if (newState == null)
                    throw new InvalidSwichedStateException
                        ("An attempt to switch to the null state was detected!");

                if (m_States.Contains(newState) == false)
                    throw new InvalidSwichedStateException
                        ("An attempt was detected to transition to a state " +
                        "that did not belong to this state machine!");
            }
            #endif

            if (CurrentState != null)
                CurrentState.Exit();

            CurrentState = newState;
            CurrentState.Enter();
        }

        public void SwichState<T>() where T : State
        {
            if (TryGetState(out T newState))
            {
                SwichState(newState);
                return;
            }

            throw new InvalidSwichedStateException("This state machine does not have a state of this type!");
        }

        public T GetState<T>() where T : State
        {
            foreach (var state in m_States)
                if (state is T tState)
                    return tState;

            return null;
        }

        public bool TryGetState<T>() where T : State
        {
            return GetState<T>() != null;
        }

        public bool TryGetState<T>(out T state) where T : State
        {
            state = GetState<T>();
            return state != null;
        }

        protected void AddState<T>() where T : State, new()
        {
            if (TryGetState<T>() == true)
                throw new InvalidAddedStateException("This state machine already " +
                    "has states of this type attached to it. " +
                    "You cannot attach the same type of state to the same machine!");

            T newState = new T();
            AddState(newState);
        }

        private void AddState(State newState)
        {
            #if UNITY_EDITOR
            {
                if (newState == null)
                    throw new InvalidAddedStateException("External attempt to add null state!");

                if (newState.StateMachine != this && newState.StateMachine != null)
                    throw new InvalidAddedStateException("Such a state is already tied to another state machine!");
            }
            #endif

            if (newState.StateMachine == null)
                newState.Bind(this);

            m_States.Add(newState);
        }
    }
}