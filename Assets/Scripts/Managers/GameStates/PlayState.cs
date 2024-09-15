using StatePattern;

namespace Managers.GameStates
{
    /// <summary>
    /// Represents the play state of the game, enabling swipe controls and displaying the HUD.
    /// </summary>
    public class PlayState : StateBase<GameManager>
    {
        public PlayState(string stateID, StateMachine<GameManager> stateMachine) : base(stateID, stateMachine)
        {
        }

        /// <summary>
        /// Called when entering the play state, enables swiping and displays the HUD.
        /// </summary>
        public override void OnEnter(GameManager context)
        {
            base.OnEnter(context);
            context.SwipesManager.enabled = true;
            context.UIManager.ChangeWindow(context.UIManager.HUD);
        }
    }
}