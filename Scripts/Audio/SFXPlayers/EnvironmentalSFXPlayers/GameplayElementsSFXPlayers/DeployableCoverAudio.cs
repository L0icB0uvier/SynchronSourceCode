using UnityEngine;

namespace Audio.SFXPlayers.EnvironmentalSFXPlayers.GameplayElementsSFXPlayers
{
    public class DeployableCoverAudio : GameplayElementAudio
    {
        [SerializeField] private AudioCueSO _deployCover;
        [SerializeField] private AudioCueSO _retractCover;

        public void PlayDeploySound() => PlayAudio(_deployCover, transform.position);
        public void PlayRetractSound() => PlayAudio(_retractCover, transform.position);
    }
}