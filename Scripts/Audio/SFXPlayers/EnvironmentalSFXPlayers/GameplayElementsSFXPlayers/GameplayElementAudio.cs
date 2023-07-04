using GeneralScriptableObjects.Events;
using UnityEngine;

namespace Audio.SFXPlayers.EnvironmentalSFXPlayers.GameplayElementsSFXPlayers
{
    public abstract class GameplayElementAudio : MonoBehaviour
    {
        [SerializeField] protected AudioCueEventChannelSO _sfxEventChannel = default;
        [SerializeField] protected AudioConfigurationSO _audioConfig = default;

        protected AudioCueKey PlayAudio(AudioCueSO audioCue, Vector3 positionInSpace = 
            default)
        {
            return _sfxEventChannel.RaisePlayEvent(audioCue, _audioConfig);
        }
        
        protected void StopAudio(AudioCueKey audioCueKey)
        {
            _sfxEventChannel.RaiseStopEvent(audioCueKey);
        }
    }
}