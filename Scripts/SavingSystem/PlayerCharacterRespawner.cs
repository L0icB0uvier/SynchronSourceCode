using System.Collections;
using GeneralScriptableObjects.Events;
using UnityEngine;
using UnityEngine.Events;

namespace SavingSystem
{
    public class PlayerCharacterRespawner : Respawner
    {
        [SerializeField] private Vector2EventChannelSO characterRespawnEvent;
        [SerializeField] private VoidEventChannelSO gameOverScreenComplete;
        
        public UnityEvent onRespawn;
        public UnityEvent onGameRestart;
        
        private void OnEnable()
        {
            characterRespawnEvent.OnEventRaised += RespawnAtLocation;
            gameOverScreenComplete.onEventRaised += GameRestart;
        }

        private void OnDisable()
        {
            characterRespawnEvent.OnEventRaised -= RespawnAtLocation;
            gameOverScreenComplete.onEventRaised -= GameRestart;
        }

        private void RespawnAtLocation(Vector2 respawnLocation)
        {
            rbd2.position = respawnLocation;
            StartCoroutine(TriggerRespawnEventWithDelay());
        }

        private void GameRestart()
        {
            onGameRestart?.Invoke();
        }

        private IEnumerator TriggerRespawnEventWithDelay()
        {
            yield return new WaitForFixedUpdate();
            onRespawn?.Invoke();
        }
    }
}