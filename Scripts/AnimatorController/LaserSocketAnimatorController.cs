using UnityEngine;

namespace AnimatorController
{
    public abstract class LaserSocketAnimatorController : AnimatorController
    {
        private static readonly int laserInteracterPresent = Animator.StringToHash("LaserInteracterPresent");
        
        public void SetLaserInteracterPresentParameter(bool present)
        {
            animator.SetBool(laserInteracterPresent, present);
        }
    }
}