using System;
using System.Collections;
using GeneralScriptableObjects.Events;
using SavingSystem;
using Sirenix.OdinInspector;
using UI.SceneSelectionWindow;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
	public class StartMenuUIManager : MonoBehaviour
	{
		[SerializeField] private GameObject m_startNewGameButton;

		[SerializeField] private GameObject m_continueButton;
		[SerializeField] private GameObject buttonsMenu;
		[SerializeField] private GameObject sceneSelectionMenu;
		[SerializeField] private GameObject settingsMenu;
		
		[Header("Broadcasting on")]
		[SerializeField] private VoidEventChannelSO startNewGameEvent = default;
		[SerializeField] private VoidEventChannelSO continueGameEvent = default;

		[SerializeField] private SaveSystem _saveSystem;
		
		private void Start()
		{
			m_continueButton.GetComponent<Button>().interactable = _saveSystem.SavedDataFound;
			InitializeButtons();
		}

		[Button]
		public void InitializeButtons()
		{
			StartCoroutine(InitialiseSelectedObject());
		}

		private IEnumerator InitialiseSelectedObject()
		{
			EventSystem.current.SetSelectedGameObject(null);
			yield return null;
			EventSystem.current.SetSelectedGameObject(_saveSystem.SavedDataFound? m_continueButton : m_startNewGameButton);
		}

		public void StartNewGame()
		{
			startNewGameEvent.onEventRaised();
		}

		public void ContinueGame()
		{
			continueGameEvent.onEventRaised();
		}

		public void SwitchToSceneSelection()
		{
			buttonsMenu.SetActive(false);
			sceneSelectionMenu.SetActive(true);
		}

		public void ReturnToMainMenu()
		{
			sceneSelectionMenu.SetActive(false);
			settingsMenu.SetActive(false);
			buttonsMenu.SetActive(true);
			
			InitializeButtons();
		}
		
		public void SwitchToSettings()
		{
			buttonsMenu.SetActive(false);
			settingsMenu.SetActive(true);
		}

		public void QuitGame()
		{
			_saveSystem.SaveSettings();
			
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
			Application.Quit();
		}
	}
}
