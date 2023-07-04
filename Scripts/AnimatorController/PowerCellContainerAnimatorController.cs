using UnityEngine;

namespace AnimatorController
{
    public class PowerCellContainerAnimatorController : TransitioningSystemAnimatorController
    {
        private static readonly int cellStored = Animator.StringToHash("CellStored");
        private static readonly int open = Animator.StringToHash("Open");
        private static readonly int close = Animator.StringToHash("Close");

        public void SetPowerCellStoredParameter(bool stored)
        {
            animator.SetBool(cellStored, stored);
        }

        public void PlayOpenContainer()
        {
            if (!CanTransitionToAltered()) return;
            animator.SetTrigger(open);
        }

        public void PlayCloseContainer()
        {
            if (!CanTransitionToDefault()) return;
            animator.SetTrigger(close);
        }
    }
}
