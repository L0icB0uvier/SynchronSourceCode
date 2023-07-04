using UnityEngine;

namespace Audio.SFXPlayers.EnvironmentalSFXPlayers.GameplayElementsSFXPlayers
{
    public class ItemContainerAudioPlayer : SFXAudioPlayer
    {
        [SerializeField] private AudioCueSO itemPluggedAudio;
        [SerializeField] private AudioCueSO itemUnpluggedAudio;

        public void PlayItemPlugged() => PlayAudio(itemPluggedAudio);
        public void PlayItemUnplugged() => PlayAudio(itemUnpluggedAudio);
    }
}