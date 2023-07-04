using System;
using System.Collections;
using GeneralScriptableObjects.Events;
using UnityEngine;
using UnityEngine.Events;

namespace SavingSystem
{
    public class LoadEventListener : MonoBehaviour
    {
        [SerializeField] private VoidEventChannelSO loadEventChannel;
        [SerializeField] private VoidEventChannelSO respawnEventChannel;
        [SerializeField] private VoidEventChannelSO initializeEventChannel;
        
        [SerializeField][Tooltip("Used to get some objects initialized later than others")] private float initializeDelay;
        
        public UnityEvent onPrepareLoad;
        public UnityEvent onLoad;
        public UnityEvent onInitialize;

        private void Awake()
        {
            loadEventChannel.onEventRaised += Load;
            respawnEventChannel.onEventRaised += LoadLevelData;
            initializeEventChannel.onEventRaised += Initialize;
        }
        
        private void OnDestroy()
        {
            loadEventChannel.onEventRaised -= Load;
            respawnEventChannel.onEventRaised -= LoadLevelData;
            initializeEventChannel.onEventRaised -= Initialize;
        }

        private void LoadLevelData()
        {
            StartCoroutine(LoadProcess());
        }

        private IEnumerator LoadProcess()
        {
            PrepareLoad();
            Load();
            yield return new WaitForSecondsRealtime(initializeDelay);
            Initialize();
        }
        
        private void PrepareLoad() => onPrepareLoad?.Invoke();

        private void Load() => onLoad?.Invoke();

        private void Initialize() => onInitialize?.Invoke();
    }
}