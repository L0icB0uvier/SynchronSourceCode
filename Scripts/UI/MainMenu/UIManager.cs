using GeneralScriptableObjects.Events;
using UnityEngine;

namespace UI.MainMenu
{
    public class UIManager : MonoBehaviour
    {
        [Header("Listening on")]
        [SerializeField] private ShowGameOverEventChannel _onGameOverEvent;

        [SerializeField] private PauseMenuOptions _pauseMenu;
        [SerializeField] private GameOverUIManager _gameOverManager;

        [SerializeField] private VoidEventChannelSO onPauseInputPressed;
        
        private void OnEnable()
        {
            _onGameOverEvent.OnEventRaised += DisplayGameOverScreen;
            onPauseInputPressed.onEventRaised += TogglePauseMenu;
        }

        private void OnDisable()
        {
            _onGameOverEvent.OnEventRaised -= DisplayGameOverScreen;
            onPauseInputPressed.onEventRaised -= TogglePauseMenu;
        }

        private void TogglePauseMenu()
        {
            switch (_pauseMenu.gameObject.activeInHierarchy)
            {
                case true:
                    _pauseMenu.ClosePauseMenu();
                    break;
                case false:
                    _pauseMenu.OpenPauseMenu();
                    break;
            }
        }

        private void DisplayGameOverScreen(Sprite sprite, bool showCharacterImage)
        {
            _gameOverManager.PlayGameOverScreen(sprite, showCharacterImage);
        }
    }
}
