using Tools.Extension;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Characters.CharacterAbilities.Movement
{
    public class BounceOff : MonoBehaviour
    {
        private Rigidbody2D m_rb2d;

        private IMovement m_movement;
        
        [SerializeField] private float minSpeedForBounceEffect = 2;
        [SerializeField] private float maxBounceSpeed = 45;

        public UnityEvent<float> onBounce;

        public LayerMask layersToBounceOffFrom;

        private void Awake()
        {
            m_rb2d = GetComponent<Rigidbody2D>();
            m_movement = GetComponent<IMovement>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!layersToBounceOffFrom.Contains(collision.gameObject)) return;
            
            if(m_rb2d.velocity.sqrMagnitude > minSpeedForBounceEffect * minSpeedForBounceEffect)
            {
                onBounce?.Invoke(MathCalculation.Remap(m_rb2d.velocity.sqrMagnitude, 0, maxBounceSpeed * 
                    maxBounceSpeed, 0.25f, .75f));
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!layersToBounceOffFrom.Contains(collision.gameObject)) return;
            
            m_movement.ChangeLookingDirection(MathCalculation.ConvertDirectionToAngle(m_rb2d.velocity.normalized));
        }
    }
}