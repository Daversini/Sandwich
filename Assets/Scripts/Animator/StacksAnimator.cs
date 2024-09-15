using System;
using Animator.AnimationStates;
using Grid;
using Managers;
using StatePattern;
using UnityEngine;

namespace Animator
{
    /// <summary>
    /// Manages the animations for stack movements, including their transitions and state management.
    /// </summary>
    public class StacksAnimator : MonoBehaviour
    {
        // Events for when the animation ends and for registering a move
        public static event Action AnimationEnded;
        public static event Action<SwipeableObject, SwipeableObject, Vector3> RegisterMove;

        public GameObject TargetStack { get => _targetStack; }
        private GameObject _targetStack;
        public Transform OriginalParent { get => _originalParent; }
        private Transform _originalParent;
        public Transform RotationPivot { get => _rotationPivot; }
        private Transform _rotationPivot;

        public Quaternion TargetRotation { get => _targetRotation; }
        private Quaternion _targetRotation;
        public Vector3 StartingPoint { get => _startingPoint; }
        private Vector3 _startingPoint;
        public Vector3 FinalPoint { get => _finalPoint; }
        private Vector3 _finalPoint;

        public float Speed { get => _speed; }
        [SerializeField]
        private float _speed;
        public float AngularSpeed { get => _angularSpeed; }
        [SerializeField]
        private float _angularSpeed;
        private float _singleStackHeight;

        private StateMachine<StacksAnimator> _stateMachine;
        public SleepState Sleep;
        public StackMoveState StackMove;
        public InvalidStackMoveState InvalidStackMove;

        private void Awake()
        {
            // Initialize the rotation pivot and state machine
            _rotationPivot = transform.GetChild(0);
            InitializeStateMachine();
        }

        private void OnEnable()
        {
            // Subscribe to events for running animations
            SwipeableObject.RunAnimation += RunAnimation;
            UndoManager.RunAnimation += RunAnimation;
        }

        private void OnDisable()
        {
            // Unsubscribe from events to avoid memory leaks
            SwipeableObject.RunAnimation -= RunAnimation;
            UndoManager.RunAnimation -= RunAnimation;
        }

        private void Update()
        {
            // Update the current state of the state machine
            _stateMachine.CurrentState.OnUpdate(this);
        }

        /// <summary>
        /// Calculates the data required for the animation, such as target position and rotation.
        /// </summary>
        private void CalculateAnimationData(string animation, SwipeableObject from, SwipeableObject to)
        {
            // Get the stack to move and its original parent
            _targetStack = from.Data.Stack;
            if (animation == InvalidStackMove.StateID)
                _originalParent = from.Data.Stack.transform.parent;
            else
                _originalParent = to.Data.Stack.transform;

            // Determine the height of a single stack element
            _singleStackHeight = _targetStack.transform.lossyScale.y;
            _rotationPivot.position = GetRotationPivotPosition(from, to);
            _targetRotation = GetTargetRotation();
            _startingPoint = GetStartingPoint(from, to);
            _finalPoint = GetFinalPoint(from, to);

            // Register the move if it's a stack move
            if (animation == Constants.STACK_MOVE) RegisterMove?.Invoke(from, to, _rotationPivot.position);
        }

        /// <summary>
        /// Starts the animation by changing the state to the specified animation state.
        /// </summary>
        private void RunAnimation(string animation, SwipeableObject from, SwipeableObject to)
        {
            // Find the appropriate state for the animation
            StateBase<StacksAnimator> animationToTrigger = null;
            foreach (var state in _stateMachine.StateList)
            {
                if (animation != state.StateID) continue;
                animationToTrigger = state;
                break;
            }

            // If the animation state is not found, log an error
            if (animationToTrigger == null)
            {
                Debug.LogError("Invalid animation");
                return;
            }

            // Calculate data and change to the animation state
            CalculateAnimationData(animation, from, to);
            _stateMachine.ChangeState(animationToTrigger);
        }

        /// <summary>
        /// Overloaded method to start the animation with custom parameters.
        /// </summary>
        private void RunAnimation(Transform parent, GameObject targetStack, Quaternion targetRotation, Vector3 pivotPosition, Vector3 startingPoint, Vector3 finalPoint)
        {
            // Set the parameters and trigger the stack move state
            _rotationPivot.SetPositionAndRotation(pivotPosition, Quaternion.identity);
            _targetStack = targetStack;
            _originalParent = parent;
            _targetRotation = targetRotation;
            _startingPoint = startingPoint;
            _finalPoint = finalPoint;
            _stateMachine.ChangeState(StackMove);
        }

        /// <summary>
        /// Invokes the AnimationEnded event to signal that the animation is complete.
        /// </summary>
        public void AnimationCompleted() => AnimationEnded?.Invoke();

        /// <summary>
        /// Calculates the pivot position used for rotation during the animation.
        /// </summary>
        private Vector3 GetRotationPivotPosition(SwipeableObject from, SwipeableObject to)
        {
            // Reset the rotation of the pivot
            _rotationPivot.rotation = Quaternion.identity;

            float rotationPivotY;
            if (from.Data.StackCount <= to.Data.StackCount)
                rotationPivotY = _singleStackHeight * to.Data.StackCount;
            else
                rotationPivotY = _singleStackHeight * from.Data.StackCount;

            // Determine the pivot position based on the swipe direction
            return SwipesManager.SwipeDirection switch
            {
                SwipeDirection.Up => new Vector3(from.transform.position.x, rotationPivotY, from.transform.position.z + GridHandler.Grid.GetCellSize() * 0.5f),
                SwipeDirection.Down => new Vector3(from.transform.position.x, rotationPivotY, from.transform.position.z - GridHandler.Grid.GetCellSize() * 0.5f),
                SwipeDirection.Right => new Vector3(from.transform.position.x + GridHandler.Grid.GetCellSize() * 0.5f, rotationPivotY, from.transform.position.z),
                SwipeDirection.Left => new Vector3(from.transform.position.x - GridHandler.Grid.GetCellSize() * 0.5f, rotationPivotY, from.transform.position.z),
                _ => Vector3.zero,
            };
        }

        /// <summary>
        /// Determines the target rotation for the stack based on the swipe direction.
        /// </summary>
        private Quaternion GetTargetRotation()
        {
            return SwipesManager.SwipeDirection switch
            {
                SwipeDirection.Up => Quaternion.Euler(-180f, 0f, 0f),
                SwipeDirection.Down => Quaternion.Euler(180f, 0f, 0f),
                SwipeDirection.Right => Quaternion.Euler(0f, 0f, 180f),
                SwipeDirection.Left => Quaternion.Euler(0f, 0f, -180f),
                _ => Quaternion.identity,
            };
        }

        /// <summary>
        /// Calculates the final position of the stack after the animation completes.
        /// </summary>
        private Vector3 GetFinalPoint(SwipeableObject from, SwipeableObject to)
        {
            return to.Data.Stack.transform.position + Vector3.up * (_singleStackHeight * (from.Data.StackCount + to.Data.StackCount - 1));
        }

        /// <summary>
        /// Determines the starting point for the stack animation based on the current stack configuration.
        /// </summary>
        private Vector3 GetStartingPoint(SwipeableObject from, SwipeableObject to)
        {
            if (from.Data.StackCount >= to.Data.StackCount)
                return from.Data.Stack.transform.position;
            else
                return from.Data.Stack.transform.position + Vector3.up * (_singleStackHeight * (to.Data.StackCount - from.Data.StackCount));
        }

        /// <summary>
        /// Initializes the state machine and sets up the initial state.
        /// </summary>
        private void InitializeStateMachine()
        {
            _stateMachine = new StateMachine<StacksAnimator>(this);
            InitializeStates();
            _stateMachine.AddState(Sleep);
            _stateMachine.AddState(StackMove);
            _stateMachine.AddState(InvalidStackMove);
            _stateMachine.RunStateMachine(Sleep, this);
        }

        /// <summary>
        /// Creates and assigns the states to be used in the state machine.
        /// </summary>
        private void InitializeStates()
        {
            Sleep = new SleepState(Constants.SLEEP, _stateMachine);
            StackMove = new StackMoveState(Constants.STACK_MOVE, _stateMachine);
            InvalidStackMove = new InvalidStackMoveState(Constants.INVALID_MOVE, _stateMachine);
        }
    }
}