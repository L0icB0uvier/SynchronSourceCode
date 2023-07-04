using System.Collections;
using GeneralScriptableObjects.Events;
using SavingSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SceneManagement
{
	public class StartGame : MonoBehaviour
	{
		[SerializeField] private LocationSO startGameLocation;
		[SerializeField] private PathSO startGamePath;
		[SerializeField] private PathStorageSO pathStorage;
		
		[SerializeField] private bool _showLoadScreen;
	
		[Header("Broadcasting on")]
		[SerializeField] private LoadEventChannelSO _loadLocation;

		[Header("Listening to")]
		[SerializeField] private VoidEventChannelSO _onNewGameButton;
		[SerializeField] private VoidEventChannelSO _onContinueButton;
		[SerializeField] private SceneSelectedEventChannel _onSceneSelected;
		
		[SerializeField] private SaveSystem _saveSystem;

		[SerializeField] private UnityEvent onGameStart;
		
		
		private void Start()
		{
			_saveSystem.LoadSavedDataFromDisk();
			_onNewGameButton.onEventRaised += StartNewGame;
			_onContinueButton.onEventRaised += ContinuePreviousGame;
			_onSceneSelected.OnEventRaised += StartAtScene;
		}

		private void OnDestroy()
		{
			_onNewGameButton.onEventRaised -= StartNewGame;
			_onContinueButton.onEventRaised -= ContinuePreviousGame;
			_onSceneSelected.OnEventRaised -= StartAtScene;
		}

		private void StartNewGame()
		{
			_saveSystem.DeleteCurrentFiles();
			
			PixelCrushers.SaveSystem.ClearSavedGameData();
			PixelCrushers.SaveSystem.DeleteSavedGameInSlot(0);

			pathStorage.lastPathTaken = startGamePath;
		
			_loadLocation.RaiseEvent(startGameLocation, _showLoadScreen);
			onGameStart?.Invoke();
		}

		private void ContinuePreviousGame()
		{
			StartCoroutine(LoadSaveGame());
		}

		private void StartAtScene(LocationSO scene, PathSO path)
		{
			_saveSystem.DeleteCurrentFiles();
			
			PixelCrushers.SaveSystem.ClearSavedGameData();
			PixelCrushers.SaveSystem.DeleteSavedGameInSlot(0);

			pathStorage.lastPathTaken = path;
			_loadLocation.RaiseEvent(scene, _showLoadScreen);
			onGameStart?.Invoke();
		}
		
		private IEnumerator LoadSaveGame()
		{
			var locationGuid = _saveSystem.locationID;
			var asyncOperationHandle = Addressables.LoadAssetAsync<LocationSO>(locationGuid);

			yield return asyncOperationHandle;

			if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
			{
				LocationSO locationSO = asyncOperationHandle.Result;
				_loadLocation.RaiseEvent(locationSO, _showLoadScreen);
				onGameStart?.Invoke();
			}
		}
	}
}
