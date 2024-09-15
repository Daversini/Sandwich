using System;
using StatePattern;
using UnityEngine;

namespace Animator.AnimationStates
{
    /// <summary>
    /// Handles the animation state for moving a stack, including the transition steps of the animation.
    /// </summary>
    public class StackMoveState : StateBase<StacksAnimator>
    {
        private Action<StacksAnimator> _performAnimation;

        public StackMoveState(string stateID, StateMachine<StacksAnimator> stateMachine) : base(stateID, stateMachine)
        {
        }

        /// <summary>
        /// Called when entering the stack move state. Initializes the animation sequence.
        /// </summary>
        public override void OnEnter(StacksAnimator context)
        {
            base.OnEnter(context);
            SoundManager.Play(Constants.SWIPE);
            // Set the parent of the target stack to the rotation pivot
            context.TargetStack.transform.parent = context.RotationPivot;
            _performAnimation = StepA;
        }

        /// <summary>
        /// Called every frame to update the animation based on the current step.
        /// </summary>
        public override void OnUpdate(StacksAnimator context)
        {
            _performAnimation?.Invoke(context);
        }

        /// <summary>
        /// Called when exiting the stack move state. Resets the stack parent and clears the animation action.
        /// </summary>
        public override void OnExit(StacksAnimator context)
        {
            base.OnExit(context);
            context.TargetStack.transform.parent = context.OriginalParent;
            _performAnimation = null;
        }

        /// <summary>
        /// First step of the animation: Moves the stack to the starting point.
        /// </summary>
        private void StepA(StacksAnimator context)
        {
            if (Vector3.Distance(context.TargetStack.transform.position, context.StartingPoint) > 0.01f)
                context.TargetStack.transform.position = Vector3.MoveTowards(context.TargetStack.transform.position, context.StartingPoint, Time.deltaTime * context.Speed);
            else
            {
                context.TargetStack.transform.position = context.StartingPoint;
                _performAnimation = StepB;
            }
        }

        /// <summary>
        /// Second step of the animation: Rotates the stack to the target rotation.
        /// </summary>
        private void StepB(StacksAnimator context)
        {
            if (Quaternion.Dot(context.RotationPivot.rotation, context.TargetRotation) < 0.99f)
                context.RotationPivot.rotation = Quaternion.RotateTowards(context.RotationPivot.rotation, context.TargetRotation, Time.deltaTime * context.AngularSpeed * 100);
            else
            {
                context.RotationPivot.rotation = context.TargetRotation;
                _performAnimation = StepC;
            }
        }

        /// <summary>
        /// Third and final step of the animation: Moves the stack to the final point and ends the animation.
        /// </summary>
        private void StepC(StacksAnimator context)
        {
            if (Vector3.Distance(context.TargetStack.transform.position, context.FinalPoint) > 0.01f)
                context.TargetStack.transform.position = Vector3.MoveTowards(context.TargetStack.transform.position, context.FinalPoint, Time.deltaTime * context.Speed);
            else
            {
                SoundManager.Play(Constants.SLICE);
                context.TargetStack.transform.position = context.FinalPoint;
                _stateMachine.ChangeState(context.Sleep);
            }
        }
    }
}