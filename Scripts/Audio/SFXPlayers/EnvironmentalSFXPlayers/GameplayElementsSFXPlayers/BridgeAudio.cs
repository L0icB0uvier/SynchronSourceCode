using UnityEngine;

namespace Audio.SFXPlayers.EnvironmentalSFXPlayers.GameplayElementsSFXPlayers
{
    public class BridgeAudio : GameplayElementAudio
    {
        [Header("Audios")]
        [SerializeField] private AudioCueSO _raiseBridge;
        [SerializeField] private AudioCueSO _lowerBridge;

        public void PlayRaiseBridgeSound() => PlayAudio(_raiseBridge, transform.position);
        public void PlayLowerBridgeSound() => PlayAudio(_lowerBridge, transform.position);
    }
}