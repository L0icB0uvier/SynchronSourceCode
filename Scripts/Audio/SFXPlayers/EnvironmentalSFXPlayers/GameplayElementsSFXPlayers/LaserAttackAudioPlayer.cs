using System;
using UnityEngine;

namespace Audio.SFXPlayers.EnvironmentalSFXPlayers.GameplayElementsSFXPlayers
{
    public class LaserAttackAudioPlayer : SFXAudioPlayer
    {
        [SerializeField] private AudioCueSO laserAudioCue;
        [SerializeField] private AudioCueSO laserImpactAudioCue;

        private AudioCueKey m_laserAudioKey;
        
        public void PlayLaserImpact()
        {
            PlayAudio(laserImpactAudioCue);
        }

        public void StopLaserSound()
        {
            StopAudio(m_laserAudioKey);
            m_laserAudioKey = AudioCueKey.Invalid;
        }

        private void OnEnable()
        {
            m_laserAudioKey = PlayAudio(laserAudioCue);
        }

        private void OnDisable()
        {
            if (m_laserAudioKey == AudioCueKey.Invalid) return;
            StopAudio(m_laserAudioKey);
        }
    }
}