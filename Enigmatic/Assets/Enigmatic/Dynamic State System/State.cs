using System;
using System.Collections.Generic;

namespace Enigmatic.DynamicStateSystem
{
    [Serializable]
    public class State
    {
        public StateMachine StateMachine { get; private set; }

        private List<State> m_States = new List<State>();

        public virtual void Bind(StateMachine stateMachine)
        {
            if (stateMachine == null)
                throw new InvalidBindingException();

            if (StateMachine != null)
                throw new InvalidBindingException();

            StateMachine = stateMachine;
        }

        public State() { }

        public State(StateMachine stateMachine)
        {
            Bind(stateMachine);
        }

        public virtual void Enter() { }

        public virtual void Tick()
        {
            foreach (var state in m_States)
                if (state.CheckConditions())
                    StateMachine.SwichState(state);
        }

        public virtual void Exit() { }

        public virtual bool CheckConditions() { return false; }

        public void AddTransition<T>() where T : State
        {
            if (StateMachine.TryGetState(out T newState))
            {
                #if DEBUG
                {
                    if (m_States.Contains(newState) == true)
                        throw new InvalidAddedTransitionException("An attempt to re-add a state transition was detected. " +
                            "In this state there is already a transition to a state of this type");
                }
                #endif

                m_States.Add(newState);
                return;
            }

            throw new InvalidAddedTransitionException("An attempt was detected to add a transition " +
                            "between states belonging to different state machines");
        }
    }
}