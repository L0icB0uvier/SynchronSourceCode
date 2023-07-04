using UnityEngine;

namespace AnimatorController
{
    public class SimpleLaserTransmitterSocketAnimatorController : LaserSocketAnimatorController
    {
        private static readonly int transmitterPlugged = Animator.StringToHash("TransmitterPlugged");
        private static readonly int objectInteracterPresent = Animator.StringToHash("ObjectInteracterPresent");
        
        public void SetTransmitterPluggedParameter(bool plugged)
        {
            animator.SetBool(transmitterPlugged, plugged);
        }

        public void SetObjectInteracterPresentParameter(bool present)
        {
            animator.SetBool(objectInteracterPresent, present);
        }
    }
}