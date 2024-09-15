using System;
using StatePattern;
using UnityEngine;

namespace Animator.AnimationStates
{
    /// <summary>
    /// Represents the state of an invalid stack move animation, handling the steps for the animation sequence.
    /// </summary>
    public class InvalidStackMoveState : StateBase<StacksAnimator>
    {
        private Action<StacksAnimator> _performAnimation;
        private Vector3 _point1;
        private Vector3 _point2;

        public InvalidStackMoveState(string stateID, StateMachine<StacksAnimator> stateMachine) : base(stateID, stateMachine)
        {
        }

        /// <summary>
        /// Called when entering the state, initializing the animation parameters.
        /// </summary>
        public override void OnEnter(StacksAnimator context)
        {
            base.OnEnter(context);
            SoundManager.Play(Constants.SWIPE);
            context.TargetStack.transform.parent = context.RotationPivot;
            _point2 = context.TargetStack.transform.position;
            _performAnimation = StepA;
        }

        /// <summary>
        /// Called every frame while in this state, performs the current step of the animation.
        /// </summary>
        public override void OnUpdate(StacksAnimator context)
        {
            _performAnimation?.Invoke(context);
        }

        /// <summary>
        /// Called when exiting the state, resetting the animation parameters.
        /// </summary>
        public override void OnExit(StacksAnimator context)
        {
            base.OnExit(context);
            SoundManager.Play(Constants.SLICE);
            context.TargetStack.transform.parent = context.OriginalParent;
            _performAnimation = null;
        }

        /// <summary>
        /// First step of the animation: moves the stack to the starting point.
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
        /// Second step of the animation: rotates the pivot to the target rotation.
        /// </summary>
        private void StepB(StacksAnimator context)
        {
            if (Quaternion.Dot(context.RotationPivot.rotation, context.TargetRotation) < 0.99f)
                context.RotationPivot.rotation = Quaternion.RotateTowards(context.RotationPivot.rotation, context.TargetRotation, Time.deltaTime * context.AngularSpeed * 100);
            else
            {
                context.RotationPivot.rotation = context.TargetRotation;
                _point1 = context.TargetStack.transform.position;
                _performAnimation = StepC;
            }
        }

        /// <summary>
        /// Third step of the animation: moves the stack to the final point.
        /// </summary>
        private void StepC(StacksAnimator context)
        {
            if (Vector3.Distance(context.TargetStack.transform.position, context.FinalPoint) > 0.01f)
                context.TargetStack.transform.position = Vector3.MoveTowards(context.TargetStack.transform.position, context.FinalPoint, Time.deltaTime * context.Speed);
            else
            {
                SoundManager.Play(Constants.SLICE);
                context.TargetStack.transform.position = context.FinalPoint;
                _performAnimation = StepD;
            }
        }

        /// <summary>
        /// Fourth step of the animation: moves the stack back to the point1.
        /// </summary>
        private void StepD(StacksAnimator context)
        {
            if (Vector3.Distance(context.TargetStack.transform.position, _point1) > 0.01f)
                context.TargetStack.transform.position = Vector3.MoveTowards(context.TargetStack.transform.position, _point1, Time.deltaTime * context.Speed);
            else
            {
                context.TargetStack.transform.position = _point1;
                _performAnimation = StepE;
            }
        }

        /// <summary>
        /// Fifth step of the animation: rotates the pivot back to the initial rotation (identity).
        /// </summary>
        private void StepE(StacksAnimator context)
        {
            if (Quaternion.Dot(context.RotationPivot.rotation, Quaternion.identity) < 0.99f)
                context.RotationPivot.rotation = Quaternion.RotateTowards(context.RotationPivot.rotation, Quaternion.identity, Time.deltaTime * context.AngularSpeed * 100);
            else
            {
                context.RotationPivot.rotation = Quaternion.identity;
                _performAnimation = StepF;
            }
        }

        /// <summary>
        /// Final step of the animation: moves the stack back to point2 and changes the state to sleep.
        /// </summary>
        private void StepF(StacksAnimator context)
        {
            if (Vector3.Distance(context.TargetStack.transform.position, _point2) > 0.01f)
                context.TargetStack.transform.position = Vector3.MoveTowards(context.TargetStack.transform.position, _point2, Time.deltaTime * context.Speed);
            else
            {
                context.TargetStack.transform.position = _point2;
                _stateMachine.ChangeState(context.Sleep);
            }
        }
    }
}