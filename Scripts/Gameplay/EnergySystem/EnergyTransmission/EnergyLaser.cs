using System.Collections.Generic;
using GeneralScriptableObjects;
using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Gameplay.EnergySystem.EnergyTransmission
{
    public class EnergyLaser : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField] private Transform originLaserCap;
        [SerializeField] private Transform destinationLaserCap;
        [SerializeField] private SpriteRenderer laserContactVisual;
        
        [FoldoutGroup("Settings")]
        public float laserYOffset = 1f;

        private bool m_laserActive;

        [SerializeField] private LayerMask laserHitLayerMask;
        
        [SerializeField] private LayerMask transmitterLayerMask;
        [SerializeField] private LayerMask characterLayerMask;
        
        [SerializeField] private string[] obstacleLayers;

        [SerializeField] private FloatVariable contactVisualMaxAlpha;
        [SerializeField] private FloatVariable contactVisualMinAlpha;
        
        ITransmitLaser m_transmittingSocket;
        public ITransmitLaser TransmittingSocket => m_transmittingSocket;
        public IReceiveLaser m_receivingSocket;

        [SerializeField] private float lineCastOffset = 1;
        
        private RaycastHit2D[] hits = new RaycastHit2D[5];
        
        [SerializeField] private EdgeCollider2D laserCollider;

        private Vector2[] m_laserPointsWorldPos = new Vector2[2];
        
        private Collider2D m_colliderHit;
        private bool m_laserReceiverReached;
        
        [SerializeField] private float lineRendererOffset = .1f;

        [SerializeField] private UnityEvent<Vector2> onLaserEmissionStart;
        [SerializeField] private UnityEvent onLaserEmissionEnd;

        private Vector2 dir;
        
        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            laserCollider = GetComponent<EdgeCollider2D>();
        }
        
        public void UpdateLaser()
        {
            //check for hit
            if(HitSomething(out var hit))
            {
                //Hit a transmitter
                if (transmitterLayerMask == (transmitterLayerMask | 1 << hit.collider.transform.gameObject.layer))
                {
                    RetargetLaser(hit.collider.GetComponentInParent<IReceiveLaser>());
                    return;
                }
                
                //Hit an obstacle
                if ((LayerMask.GetMask(obstacleLayers) & 1 << hit.collider.transform.gameObject.layer) != 0)
                {
                    if (m_colliderHit != null && m_colliderHit == hit.collider) return;
                    LaserGotBlocked(hit);
                }
 
                if (characterLayerMask == (characterLayerMask | 1 << hit.collider.transform.gameObject.layer))
                {
                    ShowContactSprite(hit);
                }
                
                else
                {
                    if (laserContactVisual.transform.parent.gameObject.activeInHierarchy)
                    {
                        laserContactVisual.transform.parent.gameObject.SetActive(false);
                    }
                }
            }
            
            else
            {
                if (laserContactVisual.transform.parent.gameObject.activeInHierarchy)
                {
                    laserContactVisual.transform.parent.gameObject.SetActive(false);
                }
                
                LaserReachReceiver();
            }
        }

        private void ShowContactSprite(RaycastHit2D hit)
        {
            var position = hit.collider.transform.position;
            var distanceToColliderCenter = Vector2.Distance(hit.point, position);
            var dirToCenter = ((Vector2)position - hit.point).normalized;
            float angleToCenter = MathCalculation.ConvertDirectionToAngle(dirToCenter);
            float laserAngle = MathCalculation.ConvertDirectionToAngle(dir);
            float alphaAngle = Mathf.Abs(Mathf.DeltaAngle(laserAngle, angleToCenter));
            float distanceToContact = Mathf.Cos(alphaAngle * Mathf.Deg2Rad) * distanceToColliderCenter;

            Vector2 contactPosition = hit.point + (dir * distanceToContact);
            laserContactVisual.transform.parent.position = contactPosition;
            laserContactVisual.transform.eulerAngles = new Vector3(0, 0, MathCalculation.ConvertDirectionToAngle(dir));
            float angleFactor = Mathf.Cos(alphaAngle * Mathf.Deg2Rad);
            float visualAlpha = Mathf.Lerp(contactVisualMinAlpha.Value, contactVisualMaxAlpha.Value, angleFactor);
            
            Color laserColor = new Color(1, 1, 1, visualAlpha);
            laserContactVisual.color = laserColor;

            if (!laserContactVisual.transform.parent.gameObject.activeInHierarchy)
            {
                laserContactVisual.transform.parent.gameObject.SetActive(true);
            }
        }

        private void RetargetLaser(IReceiveLaser receiver)
        {
            m_receivingSocket.LoseLaser();
            m_receivingSocket.IncomingLaserDisconnected();
            m_receivingSocket = receiver;
            m_receivingSocket.IncomingLaserConnected(this);
            SetLaserEndPos(m_receivingSocket.ReceiverTransform.position);
            UpdateLineRenderer();
            UpdateLaserCollider();
        }

        private bool HitSomething(out RaycastHit2D raycastHit)
        {
            var startPos = m_transmittingSocket.TransmitterTransform.position;
            var endPos = m_receivingSocket.ReceiverTransform.position;
            dir = MathCalculation.GetDirectionalVectorBetween2Points(startPos,
                endPos);

            Vector2 raycastStart = (Vector2)startPos + dir * lineCastOffset;
            Vector2 raycastEnd = (Vector2)endPos - dir * lineCastOffset;

            int hitsCount = Physics2D.LinecastNonAlloc(raycastStart, raycastEnd, hits, laserHitLayerMask.value);
            if(hitsCount > 1)
            {
                for (int i = 0; i < hitsCount; i++)
                {
                    if (hits[i].collider == null) break;

                    if (hits[i].collider == laserCollider) continue;
                    
                    raycastHit = hits[i];
                    return true;
                }
            }
            
            raycastHit = new RaycastHit2D();
            return false;
        }

        private void LaserReachReceiver()
        {
            if (m_laserReceiverReached == false)
            {
                SetLaserEndPos(m_receivingSocket.ReceiverTransform.position);
                UpdateLineRenderer();
                UpdateLaserCollider();
                m_colliderHit = null;
                m_laserReceiverReached = true;
            }
            
            m_receivingSocket.ReceiveLaser();
        }

        private void LaserGotBlocked(RaycastHit2D hit)
        {
            m_laserReceiverReached = false;
            m_colliderHit = hit.collider;
            SetLaserEndPos(hit.point);
            UpdateLineRenderer();
            if(m_laserActive == false) Activate();
            UpdateLaserCollider();
            m_receivingSocket.LoseLaser();
        }

        public void Initialise(ITransmitLaser transmitter, IReceiveLaser receiver)
        {
            m_transmittingSocket = transmitter;
            m_receivingSocket = receiver;
        
            SetLaserStartPos(m_transmittingSocket.TransmitterTransform.position);
            SetLaserEndPos(m_receivingSocket.ReceiverTransform.position);
            
            UpdateLaser();
        }

        public void Activate()
        {
            SetLaserStartPos(m_transmittingSocket.TransmitterTransform.position);
            SetLaserEndPos(m_receivingSocket.ReceiverTransform.position);
            onLaserEmissionStart?.Invoke( m_laserPointsWorldPos[0]);
            UpdateLaser();
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            onLaserEmissionEnd?.Invoke();
            
            m_receivingSocket.LoseLaser();
        }

        public void DespawnLaser()
        {
            Deactivate();
            
            m_transmittingSocket.OutgoingLaserDisconnected();
            m_receivingSocket.IncomingLaserDisconnected();

            m_transmittingSocket = null;
            m_receivingSocket = null;

            m_colliderHit = null;
            m_laserReceiverReached = false;
            
            LeanPool.Despawn(gameObject);
        }

        private void SetLaserEndPos(Vector2 laserPos)
        {
            m_laserPointsWorldPos[1] = laserPos;
        }

        private void UpdateLineRenderer()
        {
            var dir = GetLaserDirection();
            var laserStartPos = m_laserPointsWorldPos[0] + dir * lineRendererOffset + new Vector2(0, laserYOffset);
            var laserEndPos = m_laserPointsWorldPos[1] - dir * lineRendererOffset + new Vector2(0, laserYOffset);
            lineRenderer.SetPosition(0, laserStartPos);
            lineRenderer.SetPosition(1, laserEndPos);

            originLaserCap.position = laserStartPos;
            destinationLaserCap.position = laserEndPos;
        }

        private void SetLaserStartPos(Vector2 laserPos)
        {
            m_laserPointsWorldPos[0] = laserPos;
        }

        private Vector2 GetLaserDirection()
        {
            return MathCalculation.GetDirectionalVectorBetween2Points(m_laserPointsWorldPos[0],
                m_laserPointsWorldPos[1]);
        }

        private void UpdateLaserCollider()
        {
            var edgeColliderPoints = new List<Vector2>();
            
            var dir = GetLaserDirection();

            edgeColliderPoints.Add((Vector2)transform.InverseTransformPoint(m_laserPointsWorldPos[0]) + dir * .1f);
            edgeColliderPoints.Add((Vector2)transform.InverseTransformPoint(m_laserPointsWorldPos[1]) - dir * .1f);

            laserCollider.SetPoints(edgeColliderPoints);
        }
    }
}