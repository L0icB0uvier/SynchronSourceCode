using System.Collections;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using GeneralScriptableObjects.SceneData;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
	public class PauseMenuOptions : MonoBehaviour
	{
		[SerializeField] private GameObject pauseMenuFirstButton;

		[SerializeField] private MenuSO _mainMenu;
		
		[Header("Broadcasting on")]
		[SerializeField] private BoolEventChannelSO changeTimePausedEvent;
		[SerializeField] private LoadEventChannelSO _loadMenuEvent = default;
		[SerializeField] private VoidEventChannelSO restartLevelChannel;
		[SerializeField] private VoidEventChannelSO startRespawnTransitionChannel;
		
		[SerializeField] private FadeChannelSO _fadeChannel;

		[SerializeField] private bool showLoadingScreenOnReturnToMainMenu = false;
		[SerializeField] private FloatVariable fadeDuration;

		public UnityEvent onCloseMenu;

		public void OpenPauseMenu()
		{
			gameObject.SetActive(true);
			
			EventSystem.current.SetSelectedGameObject(null);
			EventSystem.current.SetSelectedGameObject(pauseMenuFirstButton);
			
			changeTimePausedEvent.RaiseEvent(true);
		}

		public void ClosePauseMenu()
		{
			onCloseMenu?.Invoke();
			gameObject.SetActive(false);
			changeTimePausedEvent.RaiseEvent(false);
		}
		
		public void Resume()
		{
			ClosePauseMenu();
		}

		public void ReturnToMainMenu()
		{
			ClosePauseMenu();
			_loadMenuEvent.RaiseEvent(_mainMenu, showLoadingScreenOnReturnToMainMenu, true);
		}

		public void RestartFromCheckPoint()
		{
			StartCoroutine(RestartLevel());
		}

		private IEnumerator RestartLevel()
		{
			_fadeChannel.FadeOut(fadeDuration.Value);
			yield return new WaitForSecondsRealtime(fadeDuration.Value);
			restartLevelChannel.RaiseEvent();
			startRespawnTransitionChannel.RaiseEvent();
			ClosePauseMenu();
		}
		
		
	}
}
