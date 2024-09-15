using System.Collections.Generic;

namespace StatePattern
{

    /// <summary>
    /// State manager to handle basic states
    /// </summary>
    /// <typeparam name="TContext">The type of the context</typeparam>
    public class StateMachine<TContext> 
    {
        public TContext _context;
        public StateBase<TContext> CurrentState;
        public StateBase<TContext> PreviousState;
        public List<StateBase<TContext>> StateList;

        public StateMachine(TContext context)
        {
            _context = context;
        }

        public void RunStateMachine(StateBase<TContext> entryPoint, TContext context)
        {
            //StateBase<TContext>.OnChangeState += ChangeState;
            _context = context;
            CurrentState = entryPoint;
            CurrentState.OnEnter(_context);
        }

        public void AddState(StateBase<TContext> state)
        {
            StateList = (StateList == null) ? new List<StateBase<TContext>>() : StateList;
            StateList.Add(state);
        }

        /// <summary>
        /// Changes the current state to the passed state (only if the current state is not already the passed state)
        /// </summary>
        public void ChangeState(StateBase<TContext> state)
        {
            if (CurrentState == state) return;

            PreviousState = CurrentState;
            CurrentState.OnExit(_context);
            CurrentState = state;
            CurrentState.OnEnter(_context);
        }
    }
}


