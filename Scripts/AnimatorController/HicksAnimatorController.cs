using GeneralScriptableObjects;
using UnityEngine;

namespace AnimatorController
{
    public class HicksAnimatorController : PlayerAnimatorController
    {
        [SerializeField] private BoolVariable stealthModeActive;
        [SerializeField] private BoolVariable moving;
        
        private static readonly int stealth = Animator.StringToHash("Stealth");
        private static readonly int isMoving = Animator.StringToHash("IsMoving");
        
        protected override void SetAnimatorParameters()
        {
            base.SetAnimatorParameters();
            animator.SetBool(stealth, stealthModeActive.Value);
            animator.SetBool(isMoving, moving.Value);
        }
    }
}