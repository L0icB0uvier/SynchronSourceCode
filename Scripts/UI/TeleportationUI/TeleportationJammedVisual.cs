using UnityEngine;

namespace UI
{
    public class TeleportationJammedVisual : MonoBehaviour
    {
        private Animator m_animator;
        private SpriteRenderer m_spriteRenderer;

        public void ChangeToTeleportPossibleColor()
        {
            
        } 
        
        public void ChangeToTeleportImpossibleColor()
        {
            
        }
        
        private void Awake()
        {
            m_animator = GetComponent<Animator>();
            m_spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void PlayTeleportationPossible()
        {
            m_animator.Play("TeleportationPossible");
        }

        public void PlayTeleportationJammed()
        {
            m_animator.Play("TeleportationJammed");
        }

        public void OnTeleportationFailed()
        {
            m_animator.Play("TeleportationFailed");
        }

        public void ChangeTransparency(float a)
        {
            var color = m_spriteRenderer.color;
            Color newColor = new Color(color.r, color.b, color.g, a);
            color = newColor;
            m_spriteRenderer.color = color;
        }
    }
}
