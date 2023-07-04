using System;
using GeneralScriptableObjects.Events;
using UnityEngine;
using UnityEngine.Events;

public class DeathCameraEnabler : MonoBehaviour
{
    [SerializeField] private Vector2EventChannelSO onCharacterDiedEventChannel;
    [SerializeField] private VoidEventChannelSO onRespawnEventChannel;
    
    [SerializeField] private UnityEvent onCharacterDied;
    [SerializeField] private UnityEvent onGameRestart;

    private void Awake()
    {
        onCharacterDiedEventChannel.OnEventRaised += OnCharacterDied;
        onRespawnEventChannel.onEventRaised += OnRespawn;
    }

    private void OnDestroy()
    {
        onCharacterDiedEventChannel.OnEventRaised -= OnCharacterDied;
        onRespawnEventChannel.onEventRaised -= OnRespawn;
    }

    private void OnCharacterDied(Vector2 characterLocation)
    {
        var transform1 = transform;
        transform1.position = new Vector3(characterLocation.x, characterLocation.y, transform1.position.z);
        onCharacterDied?.Invoke();
    }

    private void OnRespawn()
    {
        onGameRestart?.Invoke();
    }
}
