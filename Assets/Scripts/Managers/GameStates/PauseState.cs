using StatePattern;
using UnityEngine;

namespace Managers.GameStates
{
    /// <summary>
    /// Represents the paused state of the game, disabling swipe controls and displaying the pause menu.
    /// </summary>
    public class PauseState : StateBase<GameManager>
    {
        public PauseState(string stateID, StateMachine<GameManager> stateMachine) : base(stateID, stateMachine)
        {
        }

        /// <summary>
        /// Called when entering the pause state, disables swiping and displays the pause menu.
        /// </summary>
        public override void OnEnter(GameManager context)
        {
            base.OnEnter(context);
            context.SwipesManager.enabled = false;
            context.UIManager.ChangeWindow(context.UIManager.Menu);
        }

        /// <summary>
        /// Called every frame to check if the game should resume.
        /// </summary>
        public override void OnUpdate(GameManager context)
        {
            ChecksForResumeGame(context);
        }

        /// <summary>
        /// Checks for a right swipe gesture to resume the game from the paused state.
        /// </summary>
        private void ChecksForResumeGame(GameManager context)
        {
            if (Input.touchCount == 0) return;

            Touch touch = Input.GetTouch(0);
            // Checks if the swipe is to the right and has enough magnitude to count as a gesture to resume the game
            if (Vector2.Dot(touch.deltaPosition.normalized, Vector2.right) > 0.99f && touch.deltaPosition.magnitude > 30f)
            {
                SoundManager.Play(Constants.SWIPE);
                _stateMachine.ChangeState(context.Play);
            }
        }
    }
}