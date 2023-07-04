using UnityEngine;

namespace AnimatorController
{
    public class LaserSourceSocketAnimatorController : LaserSocketAnimatorController
    {
        private static readonly int sourcePowered = Animator.StringToHash("Powered");
        
        public void SetSourcePoweredParameter(bool powered)
        {
            animator.SetBool(sourcePowered, powered);
        }
    }
}