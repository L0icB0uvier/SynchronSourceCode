using Gameplay.PoweredObjects.ControlledPoweredObjects;
using TMPro;
using UnityEngine;

namespace UI
{
    public class InteractionHollogram : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private static readonly int fadeIn = Animator.StringToHash("FadeIn");
        private static readonly int fadeOut = Animator.StringToHash("FadeOut");
        private static readonly int interacted = Animator.StringToHash("Interacted");
        private static readonly int interactionFinished = Animator.StringToHash("InteractionFinished");
        private static readonly int _powered = Animator.StringToHash("Powered");

        private MovingPoweredSystem m_movingPoweredSystem;

        public void ChangePoweredState(bool powered)
        {
            _animator.SetBool(_powered , powered);
        }

        public void ShowHollogram(MovingPoweredSystem movingPoweredSystem)
        {
            _animator.SetTrigger(fadeIn);
            m_movingPoweredSystem = movingPoweredSystem;
            ChangePoweredState(movingPoweredSystem.Powered);
            m_movingPoweredSystem.onPowerStateChanged += ChangePoweredState;
        }

        public void HideHollogram()
        {
            _animator.SetTrigger(fadeOut);
            m_movingPoweredSystem.onPowerStateChanged -= ChangePoweredState;
            m_movingPoweredSystem = null;
        }

        public void PlayInteracted()
        {
            _animator.SetTrigger(interacted);
        }

        public void PlayIdle()
        {
            _animator.SetTrigger(interactionFinished);
        }
    }
}