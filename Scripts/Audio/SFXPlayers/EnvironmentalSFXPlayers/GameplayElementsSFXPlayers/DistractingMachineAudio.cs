using UnityEngine;

namespace Audio.SFXPlayers.EnvironmentalSFXPlayers.GameplayElementsSFXPlayers
{
    public class DistractingMachineAudio : GameplayElementAudio
    {
        [SerializeField] private AudioCueSO machineOn;

        private AudioCueKey m_machineOnKey;

        private void Awake()
        {
            m_machineOnKey = AudioCueKey.Invalid;
        }

        public void PlayMachineNoise()
        {
            if (m_machineOnKey != AudioCueKey.Invalid) return;
            
            m_machineOnKey = PlayAudio(machineOn, transform.position);
        }

        public void StopMachineNoise()
        {
            if (m_machineOnKey == AudioCueKey.Invalid) return;
            
            StopAudio(m_machineOnKey);
            m_machineOnKey = AudioCueKey.Invalid;
        }

        private void OnDestroy()
        {
            if (m_machineOnKey == AudioCueKey.Invalid) return;
            StopMachineNoise();
        }
    }
}