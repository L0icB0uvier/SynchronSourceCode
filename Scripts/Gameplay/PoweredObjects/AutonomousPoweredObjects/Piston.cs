using System;
using Characters.DamageSystem;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.PoweredObjects.AutonomousPoweredObjects
{
    public class Piston : PoweredSystem
    {
        private GraphUpdateObject m_guo;

        [SerializeField][Range(0, 1)][OnValueChanged("SetCycleStart")][FoldoutGroup("Settings")]
        private float cycleOffset;
    
        [SerializeField][FoldoutGroup("Settings")]
        private float closingTime;
    
        [SerializeField][FoldoutGroup("Settings")]
        private AnimationCurve closingCurve;
    
        [SerializeField][FoldoutGroup("Settings")]
        private float openingTime;
    
        [SerializeField][FoldoutGroup("Settings")]
        private AnimationCurve openingCurve;
    
        [SerializeField][FoldoutGroup("Settings")]
        private float closedPauseTime;
    
        [SerializeField][FoldoutGroup("Settings")]
        private float openedPauseTime;
    
        private float m_currentTime;
        private float m_t;
    
        [SerializeField][FoldoutGroup("Settings/Constant")]
        private Vector2 closedPosition;
    
        private Vector2 m_closedPositionWS;
    
        [SerializeField][FoldoutGroup("Settings/Constant")]
        private Vector2 openedPosition;
    
        private Vector2 m_openedPositionWS;

        [SerializeField][FoldoutGroup("Settings/Constant")]
        private Vector2 damageCenter;
    
        [SerializeField][FoldoutGroup("Settings/Constant")]
        private LayerMask damageableContactFilter;
    
        [SerializeField][MinMaxSlider(0, 1, true)][FoldoutGroup("Settings/Constant")]
        private Vector2 damageWindow;

        public enum EPistonState
        {
            Opened, Closed, Opening, Closing
        }

        [FoldoutGroup("Settings/Constant")][PropertyOrder(1)]
        public Bounds elementBounds;

        [SerializeField][FoldoutGroup("References")]
        private Rigidbody2D pistonRb2D;
    
        [SerializeField][ReadOnly][FoldoutGroup("Debug")]
        private EPistonState currentState;
    
        Collider2D[] contacts = new Collider2D[5];

        protected void Awake()
        {
            m_closedPositionWS = transform.TransformPoint(closedPosition);
            m_openedPositionWS = transform.TransformPoint(openedPosition);
        
            elementBounds.center = m_closedPositionWS;
            m_guo = new GraphUpdateObject(elementBounds) {resetPenaltyOnPhysics = false};
        }

        protected void Start()
        {
            SetCycleStart();
        }

        private void SetCycleStart()
        {
            m_closedPositionWS = transform.TransformPoint(closedPosition);
            m_openedPositionWS = transform.TransformPoint(openedPosition);
        
            var cycleLenght = openedPauseTime + closingTime + closedPauseTime + openingTime;
            var offsetTime = cycleLenght * cycleOffset;

            if (offsetTime < openedPauseTime)
            {
                currentState = EPistonState.Opened;
                m_currentTime = offsetTime;
                pistonRb2D.gameObject.transform.position =m_openedPositionWS;
                return;
            }

            if (offsetTime < openedPauseTime + closingTime)
            {
                currentState = EPistonState.Closing;
                m_currentTime = offsetTime - openedPauseTime;
                m_t = Mathf.Clamp01(m_currentTime / openingTime);
                pistonRb2D.gameObject.transform.position = Vector2.Lerp(m_openedPositionWS, m_closedPositionWS, closingCurve.Evaluate(m_t));
                return;
            }

            if (offsetTime < openedPauseTime + closingTime + closedPauseTime)
            {
                currentState = EPistonState.Closed;
                m_currentTime = offsetTime - openedPauseTime - closingTime;
                pistonRb2D.gameObject.transform.position =m_closedPositionWS;
            }

            else
            {
                currentState = EPistonState.Opening;
                m_currentTime = offsetTime - openedPauseTime - closingTime - closedPauseTime;
                m_t = Mathf.Clamp01(m_currentTime / openingTime);
                pistonRb2D.gameObject.transform.position = Vector2.Lerp(m_closedPositionWS, m_openedPositionWS, openingCurve.Evaluate(m_t));
            }
        }

        private void FixedUpdate()
        {
            switch (currentState)
            {
                case EPistonState.Opened:
                    if (!Powered) return;
                    m_currentTime += Time.fixedDeltaTime;
                    if (m_currentTime >= openedPauseTime)
                    {
                        m_currentTime = 0;
                        currentState = EPistonState.Closing;
                    }
                    break;
            
                case EPistonState.Closed:
                    if (!Powered) return;
                    m_currentTime += Time.fixedDeltaTime;
                    if (m_currentTime >= closedPauseTime)
                    {
                        m_currentTime = 0;
                        currentState = EPistonState.Opening;
                    }
                    break;
            
                case EPistonState.Opening:
                    m_currentTime += Time.fixedDeltaTime;
                    m_t = Mathf.Clamp01(m_currentTime / openingTime);
                    pistonRb2D.MovePosition(Vector2.Lerp(m_closedPositionWS, m_openedPositionWS, openingCurve.Evaluate(m_t)));
        
                    if (m_t == 1)
                    {
                        OnColliderChanged();
                        m_currentTime = 0;
                        currentState = EPistonState.Opened;
                    }
                    break;
            
                case EPistonState.Closing:
                    m_currentTime += Time.fixedDeltaTime;
                    m_t = Mathf.Clamp01(m_currentTime / closingTime);
                    pistonRb2D.MovePosition( Vector2.Lerp(m_openedPositionWS, m_closedPositionWS, closingCurve.Evaluate(m_t)));

                    if (m_t >= damageWindow.x && m_t <= damageWindow.y)
                    {
                        ApplyDamage();
                    }

                    if (m_t == 1)
                    {
                        OnColliderChanged();
                        m_currentTime = 0;
                        currentState = EPistonState.Closed;
                    
                    }
                    break;
            
                default:
                    throw new ArgumentOutOfRangeException();
            }
       
        }

        public void ApplyDamage()
        {
            if (Physics2D.OverlapCircleNonAlloc(transform.TransformPoint(damageCenter), 1.5f, contacts, damageableContactFilter) > 0)
            {
                foreach (var contact in contacts)
                {
                    if(!contact) continue;
                    var contactDamageable = contact.GetComponent<Damageable>();
                    if (!contactDamageable) continue;
                    contactDamageable.ReceiveMortalDamage();
                }
            }
        }

        public void OnColliderChanged()
        {
            AstarPath.active.UpdateGraphs(m_guo,.1f);
        }
    }
}
