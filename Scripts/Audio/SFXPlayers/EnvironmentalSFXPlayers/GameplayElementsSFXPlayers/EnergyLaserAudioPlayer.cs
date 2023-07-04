using System;
using UnityEngine;

namespace Audio.SFXPlayers.EnvironmentalSFXPlayers.GameplayElementsSFXPlayers
{
    public class EnergyLaserAudioPlayer : SFXAudioPlayer
    {
        [SerializeField] private AudioCueSO laserAudio;

        private AudioCueKey m_laserAudioKey;

        private void Awake()
        {
            m_laserAudioKey = AudioCueKey.Invalid;
        }

        public void PlayEnergyLaserAudio(Vector2 location)
        {
            if (m_laserAudioKey != AudioCueKey.Invalid) return;
            m_laserAudioKey = PlayAudio(laserAudio, location);
        }

        public void StopEnergyLaserAudio()
        {
            if (m_laserAudioKey == AudioCueKey.Invalid) return;
            StopAudio(m_laserAudioKey);
            m_laserAudioKey = AudioCueKey.Invalid;
        }

        private void OnDestroy()
        {
            StopEnergyLaserAudio();
        }
    }
}