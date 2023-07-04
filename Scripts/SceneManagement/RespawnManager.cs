using System;
using System.Collections;
using Cinemachine;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using RuntimeAnchors;
using SceneManagement.LevelManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace SceneManagement
{
    public class RespawnManager : MonoBehaviour
    {
        [FoldoutGroup("Listening Channels")][SerializeField] private VoidEventChannelSO _requestRespawnChannel;
        [FoldoutGroup("Listening Channels")][SerializeField] private VoidEventChannelSO gameOverScreenFinishedPlayingChannel;
        
        [FoldoutGroup("Broadcast Channels")]
        [FoldoutGroup("Broadcast Channels/Player Respawn Channels")][SerializeField] private Vector2EventChannelSO hicks_RespawnChannel;
        [FoldoutGroup("Broadcast Channels/Player Respawn Channels")][SerializeField] private Vector2EventChannelSO skullface_RespawnChannel;
        [FoldoutGroup("Broadcast Channels/Player Respawn Channels")][SerializeField] private VoidEventChannelSO characterRespawnedChannel;
        
        [FoldoutGroup("Broadcast Channels/Transition Channels")][SerializeField] private VoidEventChannelSO _disableLocationExitChannel;
        [FoldoutGroup("Broadcast Channels/Transition Channels")][SerializeField] private VoidEventChannelSO _startRespawnTransitionChannel;
        [FoldoutGroup("Broadcast Channels/Transition Channels")][SerializeField] private VoidEventChannelSO _sceneTransitionEndChannel;
        
        [FoldoutGroup("Broadcast Channels/Scene Data Channels")][SerializeField] private VoidEventChannelSO loadSceneDataChannel;
        [FoldoutGroup("Broadcast Channels/Scene Data Channels")][SerializeField] private VoidEventChannelSO resetAreaManagerChannel;
        [FoldoutGroup("Broadcast Channels/Scene Data Channels")][SerializeField] private VoidEventChannelSO respawnEnemiesChannel;
        
        [FoldoutGroup("Broadcast Channels/Fade")][SerializeField] private FadeChannelSO _fadeChannel;
        
        [FoldoutGroup("SO ref")][SerializeField] private FloatVariable _hicksLookingDirection;
        [FoldoutGroup("SO ref")][SerializeField] private FloatVariable _skullfaceLookingDirection;
        
        [SerializeField] private FloatVariable m_restartLevelFadeDuration;
        [FormerlySerializedAs("checkpointStorageSo")] [FormerlySerializedAs("pathStorageSo")][SerializeField] private CheckpointStorageSO _checkpointStorageSoSo;

        private Checkpoint[] m_checkpoints;
        
        private Transform m_hicksDefaultSpawnLocation;
        private Transform m_skullfaceDefaultSpawnLocation;
        
        private readonly Vector2[] m_defaultSpawnLocations = new Vector2[2];

        [SerializeField] private Vector3EventChannelSO warpCameraEventChannel;

        private void Awake()
        {
            m_checkpoints = FindObjectsOfType<Checkpoint>();
            GetDefaultLocations();
        }

        private void OnEnable()
        {
            _requestRespawnChannel.onEventRaised += Respawn;
            gameOverScreenFinishedPlayingChannel.onEventRaised += StartSceneTransition;
        }

        private void OnDisable()
        {
            _requestRespawnChannel.onEventRaised -= Respawn;
            gameOverScreenFinishedPlayingChannel.onEventRaised -= StartSceneTransition;
        }

        private void GetDefaultLocations()
        {
            m_hicksDefaultSpawnLocation = transform.GetChild(0);
            m_skullfaceDefaultSpawnLocation = transform.GetChild(1);
			
            m_defaultSpawnLocations[0] = m_hicksDefaultSpawnLocation.position;
            m_defaultSpawnLocations[1] = m_skullfaceDefaultSpawnLocation.position;
        }
        
        private Vector2[] GetPlayerCharactersRespawnLocations()
        {
            if (_checkpointStorageSoSo == null)
            {
                return m_defaultSpawnLocations;
            }
			
            //Look for the element in the available LocationEntries that matches tha last PathSO taken
            int entranceIndex = Array.FindIndex(m_checkpoints, element =>
                element.CheckpointPath == _checkpointStorageSoSo.lastCheckPoint);

            if (entranceIndex == -1)
            {
                Debug.LogWarning("The player tried to respawn in a LocationEntrance that doesn't exist, returning the default one.");
                return m_defaultSpawnLocations;
            }

            SetPlayerCharactersLookingDirection((int)m_checkpoints[entranceIndex].CheckpointLookingDirection);

            return new Vector2[]{m_checkpoints[entranceIndex].hicksRespawnLocation.position, 
                m_checkpoints[entranceIndex].skullfaceRespawnLocation.position} ;
        }
        
        private void SetPlayerCharactersLookingDirection(int destinationDirection)
        {
            _hicksLookingDirection.SetValue(destinationDirection);
            _skullfaceLookingDirection.SetValue(destinationDirection);
        }

        private void ResetSceneToSavedDataChannel()
        {
            loadSceneDataChannel.RaiseEvent();
        }

        private void RespawnEnemies()
        {
            resetAreaManagerChannel.RaiseEvent();
            respawnEnemiesChannel.RaiseEvent();
        }
        
        private IEnumerator ExecuteRespawn()
        { 
            yield return StartCoroutine(RespawnPlayerCharacters());
            RespawnEnemies();
            ResetSceneToSavedDataChannel();
        }

        private IEnumerator RespawnPlayerCharacters()
        {
            _disableLocationExitChannel.RaiseEvent();
            
            var targetGroup = FindObjectOfType<CinemachineTargetGroup>().transform;
            Vector3 targetGroupInitialPosition = targetGroup.position;

            Vector2[] respawnLocations = GetPlayerCharactersRespawnLocations();
            
            hicks_RespawnChannel.RaiseEvent(respawnLocations[0]);
            skullface_RespawnChannel.RaiseEvent(respawnLocations[1]);

            yield return null;
            
            Vector3 targetGroupNewPosition = targetGroup.position;
            Vector3 positionDelta = targetGroupNewPosition - targetGroupInitialPosition;
            warpCameraEventChannel.RaiseEvent(positionDelta);

            yield return null;
            
            characterRespawnedChannel.RaiseEvent();
        }

        private void Respawn()
        {
            StartCoroutine(ExecuteRespawn());
        }

        private void StartSceneTransition()
        {
            _sceneTransitionEndChannel.RaiseEvent();
            _fadeChannel.FadeIn(m_restartLevelFadeDuration.Value);
        }
    }
}