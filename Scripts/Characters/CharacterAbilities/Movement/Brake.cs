using System.Collections;
using UnityEngine;

namespace Characters.CharacterAbilities.Movement
{
    public class Brake : MonoBehaviour
    {
        private float m_currentBreakTime;
        [SerializeField] private float breakTime;

        private Rigidbody2D m_rb2d;
        
        private void Awake()
        {
            m_rb2d = GetComponent<Rigidbody2D>();
        }

        public void StartBreak()
        {
            m_currentBreakTime = 0;
            StartCoroutine(Braking());
        }

        private IEnumerator Braking()
        {
            Vector2 initialVelocity = m_rb2d.velocity;

            while(m_currentBreakTime < breakTime)
            {
                m_currentBreakTime += Time.fixedDeltaTime;
                float breakFactor = Mathf.Clamp01(m_currentBreakTime / breakTime);

                m_rb2d.velocity = Vector2.Lerp(initialVelocity, Vector2.zero, breakFactor);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}