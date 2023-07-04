using GeneralScriptableObjects.Events;
using GeneralScriptableObjects.SceneData;
using SavingSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Tools
{
	/// <summary>
	/// Allows a "cold start" in the editor, when pressing Play and not passing from the Initialisation scene.
	/// </summary> 
	public class EditorColdStartup : MonoBehaviour
	{
#if UNITY_EDITOR
		[SerializeField] private GameSceneSO _thisSceneSO;
		[SerializeField] private GameSceneSO _persistentManagersSO;
		[SerializeField] private AssetReference _notifyColdStartupChannel;
		[SerializeField] private LocationChangeChannelSO _locationChangedChannel;
		
		[SerializeField] private VoidEventChannelSO _onSceneReadyChannel;
		[SerializeField] private PathStorageSO _pathStorage;
		[SerializeField] private PathSO coldStartEntrancePath;
		[SerializeField] private bool resetSavedFilesOnPlay;
		

		[SerializeField] private SaveSystem saveSystem;
	
		private bool m_isColdStart;
		private void Awake()
		{
			if (SceneManager.GetSceneByName(_persistentManagersSO.sceneReference.editorAsset.name).isLoaded)
			{
				_notifyColdStartupChannel.LoadAssetAsync<LoadEventChannelSO>().Completed += OnNotifyChannelLoaded;
				return;
			}
			
			m_isColdStart = true;
			
			_locationChangedChannel.RaiseEvent(_thisSceneSO as LocationSO);

			if (coldStartEntrancePath == null)
			{
				Debug.LogWarning("There is no cold start entrance path assigned in EditorColdStartup. Assign one or the respawn manager won't be able to find a correct entrance");
			}
				
			_pathStorage.lastPathTaken = coldStartEntrancePath;
				
			if (resetSavedFilesOnPlay)
			{
				InitializeSaveFile();
			}
		}

		private void Start()
		{
			if (m_isColdStart)
			{
				_persistentManagersSO.sceneReference.LoadSceneAsync(LoadSceneMode.Additive).Completed += LoadEventChannel;
			}
		}
		
		private void InitializeSaveFile()
		{
			if (saveSystem == null) return;
			saveSystem.DeleteCurrentFiles();
			saveSystem.InitializePlayerData();
			saveSystem.SaveGameManagerData();
		}
		private void LoadEventChannel(AsyncOperationHandle<SceneInstance> obj)
		{
			_notifyColdStartupChannel.LoadAssetAsync<LoadEventChannelSO>().Completed += OnNotifyChannelLoaded;
		}

		private void OnNotifyChannelLoaded(AsyncOperationHandle<LoadEventChannelSO> obj)
		{
			if (_thisSceneSO != null)
			{
				obj.Result.RaiseEvent(_thisSceneSO);
			}
			else
			{
				//Raise a fake scene ready event, so the player is spawned
				_onSceneReadyChannel.RaiseEvent();
				//When this happens, the player won't be able to move between scenes because the SceneLoader has no conception of which scene we are in
			}
		}
#endif
	}
}
