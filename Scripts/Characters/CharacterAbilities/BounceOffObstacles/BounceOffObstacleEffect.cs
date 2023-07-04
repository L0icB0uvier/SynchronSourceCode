using Audio;
using Audio.SFXPlayers.CharacterSFXPlayers;
using Cinemachine;
using UnityEngine;

namespace Characters.CharacterAbilities.BounceOffObstacles
{
    public class BounceOffObstacleEffect : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SkullfaceAudio _hicksAudio;
        [SerializeField] private CinemachineImpulseSource bounceImpulse;
        
        private static readonly int bounced = Animator.StringToHash("Bounced");

        public void Bounce(float bounceFactor)
        {
            animator.SetTrigger(bounced);
            bounceImpulse.GenerateImpulse();
        }
    }
}