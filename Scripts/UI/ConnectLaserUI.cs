using Gameplay.EnergySystem.EnergyTransmission;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace UI
{
   public class ConnectLaserUI : MonoBehaviour
   {
      public ITransmitLaser TransmittingLaserObject { get; private set; }

      private Transform m_endTarget;

      [SerializeField] private LineRenderer sightClearLineRenderer;
      [SerializeField] private LineRenderer sightBlockedLineRenderer;
      
      [SerializeField] private ConnectLaserUISettings settings;
      [SerializeField] private ParticleSystem blockedParticleSystem;
      [SerializeField] private LayerMask obstacleLayerMask;

      private RaycastHit2D[] hit = new RaycastHit2D[1];

      private bool m_playingParticle;

      private bool m_hasConnectTarget;
      
      [FormerlySerializedAs("defaultColor")] [SerializeField] private Color clearSightColor;
      [SerializeField] private Color connectColor;
      [SerializeField] private Color blockedSightColor;

      private void Start()
      {
         ChangeLineColor(sightBlockedLineRenderer, blockedSightColor);
      }

      private void FixedUpdate()
      {
         if (IsBlockedByObstacle(out var hitPos))
         {
            blockedParticleSystem.transform.position = hitPos;
            
            if (m_playingParticle == false)
            {
                StartParticles();
            }
            
            SetClearSightLinePos(TransmittingLaserObject.TransmitterTransform.root.position, hitPos, true);
            SetBlockedSightLinePos(hitPos, m_endTarget.root.position);
            
            if(!sightBlockedLineRenderer.gameObject.activeInHierarchy) sightBlockedLineRenderer.gameObject.SetActive(true);
         }

         else
         {
            SetClearSightLinePos(TransmittingLaserObject.TransmitterTransform.root.position, m_endTarget.root.position, 
            false);
            
            if(sightBlockedLineRenderer.gameObject.activeInHierarchy) sightBlockedLineRenderer.gameObject.SetActive(false);
            
            if (m_playingParticle)
            {
               StopParticles();
            }
         }
      }

      private bool IsBlockedByObstacle(out Vector2 collisionPos)
      {
         Vector2 startPosition = TransmittingLaserObject.TransmitterTransform.root.position;
         Vector2 endPosition = m_endTarget.root.position;
         
         Vector2 dir = MathCalculation.GetDirectionalVectorBetween2Points(startPosition, endPosition);

         startPosition += dir * settings.laserSocketCollisionOffset;
         endPosition += -dir * settings.laserSocketCollisionOffset;
         
         if (Physics2D.LinecastNonAlloc(startPosition, endPosition, hit, obstacleLayerMask.value) > 0)
         {
            collisionPos = hit[0].point;
            return true;
         }
         
         collisionPos = Vector2.zero;
         return false;
      }

      private void StartParticles()
      {
         blockedParticleSystem.Play();
         m_playingParticle = true;
      }

      private void StopParticles()
      {
         blockedParticleSystem.Stop();
         m_playingParticle = false;
      }

      public void Initialise(ITransmitLaser laserTransmission, Transform target)
      {
         TransmittingLaserObject = laserTransmission;
         m_endTarget = target;
         
         ChangeLineColor(sightClearLineRenderer, clearSightColor);

         StopParticles();
         
         SetClearSightLinePos(TransmittingLaserObject.TransmitterTransform.root.position, m_endTarget.position, false);
      }

      private void SetClearSightLinePos(Vector2 startPos, Vector2 endPos, bool blocked)
      {
         Vector2 dir = MathCalculation.GetDirectionalVectorBetween2Points(startPos, endPos);

         var endOffset = blocked ? 0 : m_hasConnectTarget ? settings.laserSocketVisualOffset : settings.skullfaceOffset;
         
         sightClearLineRenderer.SetPosition(0, startPos + dir * settings.laserSocketVisualOffset);
         sightClearLineRenderer.SetPosition(1, (Vector2)endPos + -dir * endOffset);
      }
      
      private void SetBlockedSightLinePos(Vector2 startPos, Vector2 endPos)
      {
         Vector2 dir = MathCalculation.GetDirectionalVectorBetween2Points(startPos, endPos);
         
         var endOffset = m_endTarget.root.CompareTag("Skullface") ? settings.skullfaceOffset : settings.laserSocketVisualOffset;

         sightBlockedLineRenderer.SetPosition(0, (Vector2)startPos);
         sightBlockedLineRenderer.SetPosition(1, (Vector2)endPos + -dir * endOffset);
      }

      public void ChangeEndTarget(Transform newTarget, bool connectTarget)
      {
         m_endTarget = newTarget;
         m_hasConnectTarget = connectTarget;

         ChangeLineColor(sightClearLineRenderer, m_hasConnectTarget ? connectColor : clearSightColor);
      }

      private void ChangeLineColor(LineRenderer lineRenderer, Color newColor)
      {
         lineRenderer.startColor = newColor;
         lineRenderer.endColor = newColor;
      }

      public void ChangeOrigin(ITransmitLaser transmitLaser)
      {
         TransmittingLaserObject = transmitLaser;
      }
   }
}
