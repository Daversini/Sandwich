using Managers.GameStates;
using StatePattern;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Manages the overall game state, handling state transitions and managing game-related components like UI and input.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public SwipesManager SwipesManager { get => _swipesManager; }
        private SwipesManager _swipesManager;

        public UIManager UIManager { get => _uiManager; }
        private UIManager _uiManager;

        private StateMachine<GameManager> _stateMachine;
        public PauseState Pause;
        public PlayState Play;
        public WinState Win;

        private void Awake()
        {
            // Set the target frame rate and initialize essential components
            Application.targetFrameRate = 120;
            _uiManager = FindObjectOfType<UIManager>();
            _swipesManager = FindObjectOfType<SwipesManager>();
            InitializeStateMachine();
        }

        private void OnEnable()
        {
            // Subscribe to events for pausing the game and detecting a win
            HUD.PerformPause += PauseGame;
            SwipeableObject.GameWon += EnterWinState;
            
            // Subscribe to the skip event
            HUD.PerformSkip += SkipLevel;
        }

        private void OnDisable()
        {
            // Unsubscribe from events to prevent memory leaks
            HUD.PerformPause -= PauseGame;
            SwipeableObject.GameWon -= EnterWinState;
            
            // Unsubscribe from the skip event
            HUD.PerformSkip -= SkipLevel;
        }

        private void Update()
        {
            // Update the current state of the state machine
            _stateMachine.CurrentState.OnUpdate(this);
        }

        /// <summary>
        /// Initializes the state machine and sets up the game states.
        /// </summary>
        private void InitializeStateMachine()
        {
            _stateMachine = new StateMachine<GameManager>(this);
            InitializeStates();
            _stateMachine.AddState(Play);
            _stateMachine.AddState(Pause);
            _stateMachine.AddState(Win);
            _stateMachine.RunStateMachine(Pause, this); // Start with the game in pause state
        }

        /// <summary>
        /// Creates and initializes the game states.
        /// </summary>
        private void InitializeStates()
        {
            Pause = new PauseState(Constants.PAUSE, _stateMachine);
            Play = new PlayState(Constants.PLAY, _stateMachine);
            Win = new WinState(Constants.WIN, _stateMachine);
        }

        /// <summary>
        /// Transitions the game state to Pause.
        /// </summary>
        private void PauseGame()
        {
            _stateMachine.ChangeState(Pause);
        }
        
        /// <summary>
        /// Skips the current level by transitioning to the win state.
        /// </summary>
        private void SkipLevel()
        {
            EnterWinState();
        }

        /// <summary>
        /// Transitions the game state to Win.
        /// </summary>
        private void EnterWinState()
        {
            _stateMachine.ChangeState(Win);
        }
    }
}