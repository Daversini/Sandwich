using System;
using System.Collections;
using StatePattern;
using UnityEngine;

namespace Managers.GameStates
{
    /// <summary>
    /// Represents the win state of the game, displaying the win screen and handling input to proceed to the next level.
    /// </summary>
    public class WinState : StateBase<GameManager>
    {
        public static event Action LoadNextLevel;
        private bool _inputEnabled;

        public WinState(string stateID, StateMachine<GameManager> stateMachine) : base(stateID, stateMachine)
        {
        }

        /// <summary>
        /// Called when entering the win state, displays the win HUD and disables swipe controls.
        /// </summary>
        public override void OnEnter(GameManager context)
        {
            base.OnEnter(context);
            context.StartCoroutine(EnableInput(1f)); // Waits before enabling input
            context.SwipesManager.enabled = false;
            context.UIManager.ChangeWindow(context.UIManager.WinHUD);
        }

        /// <summary>
        /// Called every frame while in the win state, checks for input to proceed to the next level.
        /// </summary>
        public override void OnUpdate(GameManager context)
        {
            CheckNextLevelInput(context);
        }

        /// <summary>
        /// Called when exiting the win state, disables input and triggers the event to load the next level.
        /// </summary>
        public override void OnExit(GameManager context)
        {
            base.OnExit(context);
            DisableInput();
            LoadNextLevel?.Invoke();
        }

        /// <summary>
        /// Checks for a swipe gesture to proceed to the next level.
        /// </summary>
        private void CheckNextLevelInput(GameManager context)
        {
            if (!_inputEnabled) return;
            if (Input.touchCount == 0) return;

            Touch touch = Input.GetTouch(0);
            // Checks if the swipe is to the right with sufficient magnitude
            if (Vector2.Dot(touch.deltaPosition.normalized, Vector2.right) > 0.99f && touch.deltaPosition.magnitude > 30f)
            {
                SoundManager.Play(Constants.SWIPE);
                _stateMachine.ChangeState(context.Play);
            }
        }

        /// <summary>
        /// Enables input after a delay to prevent accidental input during transition.
        /// </summary>
        private IEnumerator EnableInput(float delay)
        {
            yield return new WaitForSeconds(delay);
            _inputEnabled = true;
        }

        /// <summary>
        /// Disables input to prevent further interactions while transitioning states.
        /// </summary>
        private void DisableInput() => _inputEnabled = false;
    }
}