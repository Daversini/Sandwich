using StatePattern;

namespace Animator.AnimationStates
{
    /// <summary>
    /// Represents the sleep state of the StacksAnimator, indicating an idle or inactive state.
    /// </summary>
    public class SleepState : StateBase<StacksAnimator>
    {
        public SleepState(string stateID, StateMachine<StacksAnimator> stateMachine) : base(stateID, stateMachine)
        {
        }

        /// <summary>
        /// Called when entering the sleep state, signaling that the animation is completed.
        /// </summary>
        public override void OnEnter(StacksAnimator context)
        {
            base.OnEnter(context);
            context.AnimationCompleted();
        }
    }
}