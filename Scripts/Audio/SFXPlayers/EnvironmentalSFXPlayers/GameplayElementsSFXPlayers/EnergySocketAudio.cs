using UnityEngine;

namespace Audio.SFXPlayers.EnvironmentalSFXPlayers.GameplayElementsSFXPlayers
{
    public class EnergySocketAudio : GameplayElementAudio
    {
        [Header("Audios")]
        [SerializeField] private AudioCueSO _openSocket;
        [SerializeField] private AudioCueSO _closeSocket;
        [SerializeField] private AudioCueSO _PlugEnergySource;
        [SerializeField] private AudioCueSO _UnplugEnergySource;

        public void PlayOpenSocketSound() => PlayAudio(_openSocket, transform.position);
        public void PlayCloseSocketSound() => PlayAudio(_closeSocket, transform.position);
        public void PlayPlugEnergySourceSound() => PlayAudio(_PlugEnergySource, transform.position);
        public void PlayUnplugEnergySourceSound() => PlayAudio(_UnplugEnergySource, transform.position);
    }
}