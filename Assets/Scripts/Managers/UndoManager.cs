using System;
using System.Collections.Generic;
using Animator;
using Data;
using Level;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Manages the undo functionality for stack movements, allowing for reversing individual or all moves.
    /// </summary>
    public class UndoManager : MonoBehaviour
    {
        public static event Action<Transform, GameObject, Quaternion, Vector3, Vector3, Vector3> RunAnimation;
        private List<InverseMove> _inverseMoves;
        private bool _canUndo;
        private bool _undoAllMoves;

        private void Awake()
        {
            // Initialize the list of inverse moves and subscribe to relevant events
            _inverseMoves = new List<InverseMove>();
            StacksAnimator.RegisterMove += RegisterMove;
            StacksAnimator.AnimationEnded += RestoreUndo;
            HUD.PerformUndo += UndoLastMove;
            HUD.PerformRestart += RestartLevel;
            LevelLoader.LevelLoaded += ResetUndoData;
        }

        private void Update()
        {
            // Continuously undo moves if _undoAllMoves is true
            if (_undoAllMoves) UndoAll();
        }

        /// <summary>
        /// Undo the last registered move, reversing the stack to its previous state.
        /// </summary>
        public void UndoLastMove()
        {
            if (_inverseMoves.Count == 0 || !_canUndo) return;

            _canUndo = false;
            InverseMove lastMove = _inverseMoves[^1];
        
            // Invoke the animation event to visually undo the move
            RunAnimation?.Invoke(lastMove.Parent, lastMove.Stack, lastMove.TargetRotation, lastMove.RotationPivotPosition, lastMove.StartingPoint, lastMove.FinalPoint);

            // Restore the stack's data to its previous state
            lastMove.Itself.Data.Stack = lastMove.Stack;
            lastMove.Itself.Data.StackCount = lastMove.StackCount;
            lastMove.Itself.Data.This = lastMove.Itself;
            lastMove.CameFrom.Data.StackCount -= lastMove.StackCount;
        
            // Remove the last move from the list
            _inverseMoves.RemoveAt(_inverseMoves.Count - 1);
        }

        /// <summary>
        /// Undo all registered moves in sequence until no moves are left.
        /// </summary>
        public void UndoAll()
        {
            if (_canUndo)
            {
                UndoLastMove();
                if (_inverseMoves.Count == 0)
                    _undoAllMoves = false;
            }
        }

        /// <summary>
        /// Registers a move to the undo stack, storing the inverse move data.
        /// </summary>
        private void RegisterMove(SwipeableObject from, SwipeableObject to, Vector3 pivotPosition)
        {
            _inverseMoves.Add(new InverseMove(from, to, GetStartingPoint(from, to), pivotPosition, GetTargetRotation()));
        }

        /// <summary>
        /// Calculates the starting point for the inverse move based on the swipeable objects involved.
        /// </summary>
        private Vector3 GetStartingPoint(SwipeableObject from, SwipeableObject to)
        {
            float singleStackHeight = from.Data.Stack.transform.lossyScale.y;
            if (from.Data.StackCount <= to.Data.StackCount)
                return to.transform.position + Vector3.up * (singleStackHeight * (from.Data.StackCount + to.Data.StackCount - 1));
            else 
                return to.transform.position + Vector3.up * (singleStackHeight * (2 * from.Data.StackCount - 1));
        }

        /// <summary>
        /// Determines the target rotation for the inverse move based on the swipe direction.
        /// </summary>
        private Quaternion GetTargetRotation()
        {
            return SwipesManager.SwipeDirection switch
            {
                SwipeDirection.Up => Quaternion.Euler(180f, 0f, 0f),
                SwipeDirection.Down => Quaternion.Euler(-180f, 0f, 0f),
                SwipeDirection.Right => Quaternion.Euler(0f, 0f, -180f),
                SwipeDirection.Left => Quaternion.Euler(0f, 0f, 180f),
                _ => Quaternion.identity,
            };
        }

        /// <summary>
        /// Triggers the undo of all moves when the level is restarted.
        /// </summary>
        private void RestartLevel() => _undoAllMoves = true;

        /// <summary>
        /// Enables the ability to perform an undo after an animation has ended.
        /// </summary>
        private void RestoreUndo() => _canUndo = true;

        /// <summary>
        /// Resets the undo data when a new level is loaded.
        /// </summary>
        private void ResetUndoData(LevelData value) => _inverseMoves = new List<InverseMove>();
    }
}