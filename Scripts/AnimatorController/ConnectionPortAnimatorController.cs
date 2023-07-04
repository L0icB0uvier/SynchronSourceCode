using UnityEngine;

namespace AnimatorController
{
    public class ConnectionPortAnimatorController : AnimatorController
    {
        private static readonly int connect = Animator.StringToHash("Connecting");
        private static readonly int disconnect = Animator.StringToHash("Disconnecting");

        [SerializeField] private SpriteRenderer m_hologramRenderer;
        
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
            
        }

        public void DisplayHologram()
        {
            m_hologramRenderer.enabled = true;
            animator.enabled = true;
        }

        public void HideHologram()
        {
            m_hologramRenderer.enabled = false;
            animator.enabled = false;
        }
    }
}