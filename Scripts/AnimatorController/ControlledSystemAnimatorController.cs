using UnityEngine;
using UnityEngine.Events;

namespace AnimatorController
{
    public class ControlledSystemAnimatorController : TransitioningSystemAnimatorController
    {
        [SerializeField] private string defaultStateName;
        [SerializeField] private string transitionToAlteredStateName;
        [SerializeField] private string alteredStateName;
        [SerializeField] private string transitionToDefaultStateName;
        
        public void PlayTransitionToAltered()
        {
            if (!CanTransitionToAltered()) return;

            var currentState = animator.GetCurrentAnimatorStateInfo(0);

            animator.PlayInFixedTime(transitionToAlteredStateName, -1, currentState.IsTag("TransitionToDefault")? 1 - currentState.normalizedTime : 0);
        }

        public void PlayTransitionToDefault()
        {
            if (!CanTransitionToDefault()) return;
            
            var currentState = animator.GetCurrentAnimatorStateInfo(0);
            
            animator.PlayInFixedTime(transitionToDefaultStateName, -1, currentState.IsTag("TransitionToAltered")? 1 - currentState.normalizedTime : 0);
        }

        public void PlayDefaultState()
        {
            animator.Play(defaultStateName);
        }

        public void PlayAlteredState()
        {
            animator.Play(alteredStateName);
        }
    }
}