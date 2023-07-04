using UnityEngine;

namespace AnimatorController
{
    public class ConnectionInterfaceAnimatorController : AnimatorController
    {
        private static readonly int connect = Animator.StringToHash("Connecting");
        private static readonly int disconnect = Animator.StringToHash("Disconnecting");
        private static readonly int interacted = Animator.StringToHash("Interacted");
        
        public void PlayConnect()
        {
            animator.Play(connect);
        }

        public void PlayDisconnect()
        {
            animator.Play(disconnect);
        }
        
        public void PlayInteract()
        {
            animator.Play(interacted);
        }
    }
}