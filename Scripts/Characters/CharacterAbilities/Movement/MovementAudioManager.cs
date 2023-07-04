using GeneralScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Characters.CharacterAbilities.Movement
{
    public class MovementAudioManager : MonoBehaviour
    {
        [SerializeField] private FloatVariable maxSpeed;
        [SerializeField] private BoolVariable isMoving;

        private Rigidbody2D m_rb2d;

        [SerializeField] private UnityEvent<float> onChangeVolume;
        
        private void Awake()
        {
            m_rb2d = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (isMoving.Value)
            {
                onChangeVolume?.Invoke(GetSpeedFactor());
            }
        }
        
        private float GetSpeedFactor()
        {
            return Mathf.Clamp01(m_rb2d.velocity.sqrMagnitude / (maxSpeed.Value * maxSpeed.Value));
        }
    }
}