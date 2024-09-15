using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Manages the UI windows in the game, allowing for transitions between different UI states.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public UIWindow HUD { get => _hud; }
        private UIWindow _hud;
    
        public UIWindow WinHUD { get => _winHud; }
        private UIWindow _winHud;
    
        public UIWindow Menu { get => _menu; }
        private UIWindow _menu;

        private UIWindow _currentWindow;

        private void Awake()
        {
            // Find and assign references to the different UI windows
            _hud = FindObjectOfType<HUD>(true);
            _winHud = FindObjectOfType<WinHUD>(true);
            _menu = FindObjectOfType<Menu>(true);
        }

        /// <summary>
        /// Changes the current active window to the specified window.
        /// </summary>
        public void ChangeWindow(UIWindow windowToOpen)
        {
            // Disable the current window if there is one
            if (_currentWindow != null)
                _currentWindow.Disable();

            // Set and enable the new window
            _currentWindow = windowToOpen;
            _currentWindow.Enable();
        }
    }
}