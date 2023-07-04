using GeneralScriptableObjects.Events;
using SceneManagement.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
	public class LevelFinishedMenu : MonoBehaviour
	{
		[SerializeField]
		GameObject m_LevelFinishedMenuFirstButton;

		[Header("Broadcasting on")]
		[SerializeField] private BoolEventChannelSO changeTimePausedEvent;

		public void DisplayLevelFinishedMenu()
		{
			changeTimePausedEvent.RaiseEvent(true);
			gameObject.SetActive(true);
			EventSystem.current.SetSelectedGameObject(m_LevelFinishedMenuFirstButton);
		}

		public void HideLevelFinishedMenu()
		{
			gameObject.SetActive(false);
			changeTimePausedEvent.RaiseEvent(false);
		}

		public void RestartGame()
		{
			//GameManager.Instance.LoadStartingScene();
		}

		public void QuitGame()
		{
			//GameManager.Instance.QuitGame();
		}
	}
}
