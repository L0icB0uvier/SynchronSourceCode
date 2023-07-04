using System;
using System.Collections;
using Characters.Controls.Controllers.PlayerControllers.Hicks;
using Characters.Controls.Controllers.PlayerControllers.Skullface;
using Cinemachine;
using GeneralEnums;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SceneManagement
{
	public class SpawnSystem : MonoBehaviour
	{
		[Header("Asset References")]
		[SerializeField] private AssetReference _RangeController;
		[SerializeField] private AssetReference _teleportUI;
		[SerializeField] private AssetReference _followConditionChecker;
		
		[Header("BroadcastingTo to")]
		[SerializeField] private VoidEventChannelSO _playerCharactersSpawnedChannel;
		[SerializeField] private VoidEventChannelSO _transitionComplete;
		[SerializeField] private Vector3EventChannelSO warpCameraEventChannel;

		[Header("Listening to")]
		[SerializeField] private VoidEventChannelSO _onSceneReady; //Raised by SceneLoader when the scene is set to active
	
		[Header("Storing in")]
		[SerializeField] private PathStorageSO _pathTaken;

		[SerializeField] private FloatVariable _hicksDefaultLookingDirection;
		[SerializeField] private FloatVariable _skullfaceDefaultLookingDirection;
		
		private LocationEntrance[] m_spawnLocations;

		[SerializeField] private EIsometricCardinal4DiagonalDirection _startingLookingDirection = EIsometricCardinal4DiagonalDirection.NorthEast;
		
		private Transform m_hicksDefaultSpawnLocation;
		private Transform m_skullfaceDefaultSpawnLocation;

		private readonly Vector2[] m_defaultSpawnLocations = new Vector2[2];

		private bool m_spawnAtDefaultLocation;

		private bool m_spawnedHappened;
		
		private void Awake()
		{
			m_spawnLocations = FindObjectsOfType<LocationEntrance>();
			GetDefaultLocations();
		}

		private void GetDefaultLocations()
		{
			m_hicksDefaultSpawnLocation = transform.GetChild(0);
			m_skullfaceDefaultSpawnLocation = transform.GetChild(1);
			
			m_defaultSpawnLocations[0] = m_hicksDefaultSpawnLocation.position;
			m_defaultSpawnLocations[1] = m_skullfaceDefaultSpawnLocation.position;
		}

		private void OnEnable()
		{
			_onSceneReady.onEventRaised += InitiateSpawn;
		}

		private void OnDisable()
		{
			_onSceneReady.onEventRaised -= InitiateSpawn;
		}

		private Vector2[] GetSpawnLocations()
		{
			if (_pathTaken == null)
			{
				m_spawnAtDefaultLocation = true;
				return m_defaultSpawnLocations;
			}
			
			//Look for the element in the available LocationEntries that matches tha last PathSO taken
			int entranceIndex = Array.FindIndex(m_spawnLocations, element =>
				element.EntrancePath == _pathTaken.lastPathTaken );

			if (entranceIndex == -1)
			{
				Debug.LogWarning("The player tried to spawn in a LocationEntry that doesn't exist, returning the default one.");
				m_spawnAtDefaultLocation = true;
				return m_defaultSpawnLocations;
			}

			return new Vector2[]{m_spawnLocations[entranceIndex].HicksEntranceLocation.position, 
			m_spawnLocations[entranceIndex].SkullfaceEntranceLocation.position} ;
		}

		private void InitiateSpawn()
		{
			if (m_spawnedHappened) return;
			m_spawnedHappened = true;
			
			StartCoroutine(Spawn());
		}

		private IEnumerator Spawn()
		{
			_RangeController.InstantiateAsync(Vector3.zero, Quaternion.identity);
			SpawnTeleportUI();
			_followConditionChecker.InstantiateAsync(Vector3.zero, Quaternion.identity);
			
			var targetGroup = Transform.FindObjectOfType<CinemachineTargetGroup>().transform;
			Vector3 targetGroupInitialPosition = targetGroup.position;
			
			Vector2[] spawnLocation = GetSpawnLocations();
			HicksPlayerController hicksInstance = FindObjectOfType<HicksPlayerController>();
			hicksInstance.transform.position = spawnLocation[0];
			SkullfacePlayerController skullfaceInstance = FindObjectOfType<SkullfacePlayerController>();
			skullfaceInstance.transform.position = spawnLocation[1];
			
			yield return null;

			Vector3 targetGroupNewPosition = targetGroup.position;
			Vector3 positionDelta = targetGroupNewPosition - targetGroupInitialPosition;
			warpCameraEventChannel.RaiseEvent(positionDelta);
			
			yield return null;
			
			_hicksDefaultLookingDirection.SetValue((int)_startingLookingDirection);
			_skullfaceDefaultLookingDirection.SetValue((int)_startingLookingDirection);
			
			if(m_spawnAtDefaultLocation)
			{
				_transitionComplete.RaiseEvent();
			}
			
			_playerCharactersSpawnedChannel.RaiseEvent();
		}

		public void SpawnTeleportUI()
		{
			_teleportUI.InstantiateAsync(Vector3.zero, Quaternion.identity);
		}
	}
}
