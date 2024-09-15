using System;
using Animator;
using Data;
using Grid;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Manages swipe detection and processing in the game, including determining the swipe direction and triggering stack movements.
    /// </summary>
    public class SwipesManager : MonoBehaviour
    {
        private Grid<SwipeableObjectData> _grid;
        private Camera _mainCamera;
        private SwipeableObject _startSwipe;
        private SwipeableObject _endSwipe;

        public static event Action<SwipeableObject, SwipeableObject> TriggerStackMovement;
        public static SwipeDirection SwipeDirection;

        private void Awake() => _mainCamera = Camera.main;

        private void OnEnable()
        {
            _grid = GridHandler.Grid;
            ResetSwipes();
            StacksAnimator.AnimationEnded += ResetSwipes;
        }

        private void OnDisable() => StacksAnimator.AnimationEnded -= ResetSwipes;

        private void Update() => SwipesCheck();

        /// <summary>
        /// Checks for swipe gestures and handles them accordingly.
        /// </summary>
        private void SwipesCheck()
        {
            if (Input.touchCount != 1) return;
            if (_startSwipe != null && _endSwipe != null) return;

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    TryCacheSwipeData(touch, out _startSwipe);
                    break;
                case TouchPhase.Ended:
                    if (_startSwipe == null) return;
                    _endSwipe = GetNeighbour(touch);
                    if (_endSwipe != null) TriggerStackMovement?.Invoke(_startSwipe, _endSwipe);
                    else ResetSwipes();
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Attempts to cache the swipe start data from the touch input.
        /// </summary>
        private void TryCacheSwipeData(Touch touch, out SwipeableObject swipeData)
        {
            Vector3 pointedWorldPosition = GetScreenToWorld(touch.position);
            if (_grid.GetGridObject(pointedWorldPosition) != null)
                swipeData = _grid.GetGridObject(pointedWorldPosition).This;
            else 
                swipeData = null;
        }

        /// <summary>
        /// Determines the neighboring object in the swipe direction from the starting point.
        /// </summary>
        private SwipeableObject GetNeighbour(Touch touch)
        {
            Vector3 swipeDirection = (GetScreenToWorld(touch.position) - _grid.GetWorldPosition(_startSwipe.Data.Column, _startSwipe.Data.Row)).normalized;
            SwipeableObjectData pointedObj = null;
            
            if (Vector3.Dot(swipeDirection, Vector3.right) > 0.8f)
            {
                pointedObj = _grid.GetGridObject(_startSwipe.Data.Row, _startSwipe.Data.Column + 1);
                SwipeDirection = SwipeDirection.Right;
            }
            else if (Vector3.Dot(swipeDirection, Vector3.left) > 0.8f)
            {
                pointedObj = _grid.GetGridObject(_startSwipe.Data.Row, _startSwipe.Data.Column - 1);
                SwipeDirection = SwipeDirection.Left;
            }
            else if ((Vector3.Dot(swipeDirection, Vector3.forward) > 0.8f))
            {
                pointedObj = _grid.GetGridObject(_startSwipe.Data.Row + 1, _startSwipe.Data.Column);
                SwipeDirection = SwipeDirection.Up;
            }   
            else if ((Vector3.Dot(swipeDirection, Vector3.back) > 0.8f))
            {
                pointedObj = _grid.GetGridObject(_startSwipe.Data.Row - 1, _startSwipe.Data.Column);
                SwipeDirection = SwipeDirection.Down;
            }

            return pointedObj?.This;
        }

        /// <summary>
        /// Converts the screen position to world position.
        /// </summary>
        private Vector3 GetScreenToWorld(Vector2 screenPos)
        {
            Ray ray = _mainCamera.ScreenPointToRay(screenPos);
            Physics.Raycast(ray, out RaycastHit hit);
            return new Vector3(hit.point.x, 0f, hit.point.z);
        }

        /// <summary>
        /// Resets the swipe data to prepare for a new swipe.
        /// </summary>
        private void ResetSwipes()
        {
            _startSwipe = null;
            _endSwipe = null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() => DisplayPointerDebug();

        /// <summary>
        /// Visualizes the swipe direction for debugging purposes.
        /// </summary>
        private void DisplayPointerDebug()
        {
            if (Input.touchCount != 1) return;

            Touch touch = Input.GetTouch(0);
            Debug.DrawRay(_mainCamera.ScreenPointToRay(touch.position).origin, _mainCamera.ScreenPointToRay(touch.position).direction * 100f, Color.green);
        }
#endif
    }

    /// <summary>
    /// Enum to define possible swipe directions.
    /// </summary>
    public enum SwipeDirection
    {
        Up,
        Down,
        Right,
        Left
    }
}