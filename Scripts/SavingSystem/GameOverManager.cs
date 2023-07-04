using System.Collections;
using GeneralScriptableObjects.Events;
using UnityEngine;

namespace SavingSystem
{
    public class GameOverManager : MonoBehaviour
    {
        [Header("Broadcast on")]
        [SerializeField] private VoidEventChannelSO _requestRespawnChannel;

        [Header("Listening to")]
        [SerializeField] private VoidEventChannelSO gameOverChannel;
        
        private void Start()
        {
            gameOverChannel.onEventRaised += GameOver;
        }
        
        private void OnDisable()
        {
            gameOverChannel.onEventRaised -= GameOver;
        }

        private void GameOver()
        {
            _requestRespawnChannel.RaiseEvent();
        }
    }
}