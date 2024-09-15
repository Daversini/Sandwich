using UnityEngine;

namespace StatePattern
{

    /// <summary>
    /// Basic state template 
    /// </summary>
    /// <typeparam name="TContext">The type of the context</typeparam>
    public class StateBase<TContext>
    {
        public string StateID;
        protected StateMachine<TContext> _stateMachine;

        public StateBase(string stateID, StateMachine<TContext> stateMachine)
        {
            StateID = stateID;
            _stateMachine = stateMachine;
        }

        public virtual void OnEnter(TContext context)
        {
            Debug.Log("OnEnter " + StateID);

        }

        public virtual void OnUpdate(TContext context)
        {
            //Debug.Log("OnUpdate " + StateID);
        }

        public virtual void OnExit(TContext context)
        {
            Debug.Log("OnExit " + StateID);
        }
    }
}

