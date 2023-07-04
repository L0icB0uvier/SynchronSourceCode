using UnityEngine;

namespace AnimatorController
{
    public abstract class TransitioningSystemAnimatorController : AnimatorController
    {
        protected bool CanTransitionToAltered()
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return !stateInfo.IsTag("Altered") && !stateInfo.IsTag("TransitionToAltered");
        }
        
        protected bool CanTransitionToDefault()
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return !stateInfo.IsTag("Default") && !stateInfo.IsTag("TransitionToDefault");
        }
    }
}