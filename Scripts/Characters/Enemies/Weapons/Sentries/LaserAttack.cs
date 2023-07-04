using System;
using System.Collections;
using Characters.DamageSystem;
using GeneralScriptableObjects.EnemyDataContainers;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Characters.Enemies.Weapons.Sentries
{
    public class LaserAttack : Damager
    {
        private LineRenderer m_lineRenderer;
        private SpriteRenderer m_laserBeam;
        private ParticleSystem m_particleSystem;
        private TrailRenderer m_trailRenderer;

        private Transform m_instigator;
        private Transform m_target;

        private bool m_laserInUse;

        [SerializeField] private LaserAttackGeneralSettings laserAttackGeneralSettings;
        
        private UnityAction m_laserOverCallback;

        private LaserAttackSettings m_laserAttackSettings;
        
        private bool m_targetHit;
        private Vector2 m_laserMovingDirection;

        private readonly Collider2D[] m_damageableHits = new Collider2D[1];

        [SerializeField] private UnityEvent onLaserStop;
        [SerializeField] private UnityEvent onLaserImpact;
        

        protected override void Awake()
        {
            base.Awake();
            m_lineRenderer = GetComponent<LineRenderer>();
            m_laserBeam = GetComponentInChildren<SpriteRenderer>();
            m_particleSystem = GetComponentInChildren<ParticleSystem>();
            m_trailRenderer = GetComponentInChildren<TrailRenderer>();
        }

        protected override void NonDamageableHit()
        {
            throw new NotImplementedException();
        }

        private void FixedUpdate()
        {
            if (!m_laserInUse) return;
            
            SetGroundParticleEffect();

            if(CheckForObstacles()) return;

            CheckForDamageables();
        }

        private void CheckForDamageables()
        {
            var size = Physics2D.OverlapCircleNonAlloc(m_laserBeam.transform.position, laserAttackGeneralSettings.collisionRadius, m_damageableHits, m_AttackContactFilter.layerMask);
            if (size > 0)
            {
                foreach (var hit in m_damageableHits)
                {
                    Damageable damageable = hit.GetComponent<Damageable>();
                    if (!damageable.CanBeDamaged) continue;
                    damageable.TakeDamage(this, (m_instigator.position - hit.transform.position).normalized);
                    onLaserImpact?.Invoke();
                    if (damageable.transform == m_target)
                    {
                        m_targetHit = true;
                    }
                }
            }
        }

        private bool CheckForObstacles()
        {
            if (Physics2D.Linecast(m_instigator.position, m_laserBeam.transform.position, laserAttackGeneralSettings.obstaclesLayerMask))
            {
                StopLaser();
                return true;
            }

            if (Physics2D.OverlapPoint(m_laserBeam.transform.position, laserAttackGeneralSettings.coverLayerMask))
            {
                StopLaser();
                return true;
            }

            return false;
        }

        private void SetGroundParticleEffect()
        {
            if (Physics2D.OverlapPoint(m_laserBeam.transform.position, laserAttackGeneralSettings.groundLayerMask))
            {
                m_laserBeam.enabled = true;
                if (!m_particleSystem.isPlaying) m_particleSystem.Play();
            }

            else
            {
                m_laserBeam.enabled = false;
                if (!m_particleSystem.isPlaying) m_particleSystem.Stop();
            }
        }
        
        public void InitialiseLaser(Transform instigator, Transform target, Vector2 laserStartOrigin, LaserAttackSettings laserAttackSettings, UnityAction onLaserOver)
        {
            StopAllCoroutines();

            m_laserAttackSettings = laserAttackSettings;
            
            m_targetHit = false;
            m_laserOverCallback = onLaserOver;
            m_target = target;
            m_instigator = instigator;
            
            Vector2 direction = (m_target.transform.position - m_instigator.position).normalized;
            
            m_lineRenderer.SetPosition(0,  (Vector2)laserStartOrigin);

            switch (laserAttackSettings.laserType)
            {
                case ELaserAttackType.Line:
                    StartCoroutine(LineLaser(direction));
                    break;
                case ELaserAttackType.Follow:
                    StartCoroutine(FollowingLaser(direction));
                    break;
                case ELaserAttackType.Circular:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void InitialiseTrail()
        {
            Gradient trailGradiant = new Gradient();
            trailGradiant.SetKeys(
                new GradientColorKey[]
                    {new GradientColorKey(laserAttackGeneralSettings.trailStartColor, 0.0f), new GradientColorKey(laserAttackGeneralSettings.trailStartColor, 1.0f)},
                new GradientAlphaKey[]
                    {new GradientAlphaKey(laserAttackGeneralSettings.trailStartAlpha, 0.0f), new GradientAlphaKey(laserAttackGeneralSettings.trailStartAlpha, 1.0f)}
            );
            m_trailRenderer.colorGradient = trailGradiant;
            m_trailRenderer.Clear();
            m_trailRenderer.enabled = true;
        }
        
        public void StopLaser()
        {
            m_laserInUse = false;
            m_lineRenderer.enabled = false;
            m_laserBeam.enabled = false;
            m_particleSystem.Stop();
            m_laserOverCallback();
            onLaserStop?.Invoke();

            StopAllCoroutines();
        
            StartCoroutine(WaitForTrail());
        }

        public void ReturnToPool()
        {
            m_laserInUse = false;
            m_lineRenderer.enabled = false;
            m_laserBeam.enabled = false;
            m_particleSystem.Stop();
            m_trailRenderer.enabled = false;
            LeanPool.Despawn(gameObject);
        }

        IEnumerator WaitForTrail()
        {
            float t = 0;
            float f = 0;
            Gradient trailGradiant = new Gradient();
            float alpha;
            Color color;

            while (t < m_trailRenderer.time - 1)
            {
                t += Time.deltaTime;
                f = Mathf.Clamp01(t / (m_trailRenderer.time - 1));
                alpha = Mathf.Lerp(laserAttackGeneralSettings.trailStartAlpha, laserAttackGeneralSettings.trailEndAlpha, f);
                color = Color.Lerp(laserAttackGeneralSettings.trailStartColor, laserAttackGeneralSettings.trailEndColor, f);
                trailGradiant.SetKeys(
                    new GradientColorKey[] {new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f)}, 
                    new GradientAlphaKey[] {new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f)}
                );
                m_trailRenderer.colorGradient = trailGradiant;
            
                yield return new WaitForEndOfFrame();
            }
        
            m_trailRenderer.enabled = false;
            LeanPool.Despawn(gameObject);
        }

        private IEnumerator LineLaser(Vector2 laserDirection)
        {
            UpdateLaserLocation((Vector2)m_instigator.position + (laserDirection * m_laserAttackSettings.laserStartOffset));
            InitialiseTrail();
            m_lineRenderer.enabled = true;
            m_laserInUse = true;
            Vector2 laserEndPos = (Vector2)m_laserBeam.transform.position + (laserDirection * m_laserAttackSettings.lineLaserMaxDistance);
       
            while (((Vector2) m_laserBeam.transform.position - laserEndPos).sqrMagnitude > 1)
            {
                UpdateLaserLocation(Vector2.MoveTowards(m_laserBeam.transform.position,
                    laserEndPos, m_laserAttackSettings.laserMovingSpeed * Time.deltaTime));
            
                yield return null;
            }
        
            StopLaser();
        } 
        
        private IEnumerator FollowingLaser(Vector2 laserDirection)
        {
            UpdateLaserLocation((Vector2)m_instigator.position + (laserDirection * m_laserAttackSettings.laserStartOffset));
            InitialiseTrail();
            m_lineRenderer.enabled = true;
            m_laserInUse = true;

            while (m_targetHit == false)
            {
                UpdateLaserLocation(Vector2.MoveTowards(m_laserBeam.transform.position,
                    (Vector2)m_target.position, m_laserAttackSettings.laserMovingSpeed * Time.fixedDeltaTime));
            
                yield return new WaitForFixedUpdate();
            }
            
            Vector2 laserEndPos = (Vector2)m_laserBeam.transform.position + (m_laserMovingDirection * m_laserAttackSettings.followLaserTargetHitContinueDistance);
            
            while (((Vector2) m_laserBeam.transform.position - laserEndPos).sqrMagnitude > 1)
            {
                UpdateLaserLocation(Vector2.MoveTowards(m_laserBeam.transform.position,
                    laserEndPos, m_laserAttackSettings.laserMovingSpeed * Time.deltaTime));
            
                yield return null;
            }
            
            StopLaser();
        }
        
        private void UpdateLaserLocation(Vector2 location)
        {
            m_laserMovingDirection = MathCalculation.GetDirectionalVectorBetween2Points(m_laserBeam.transform.position, location);
            m_lineRenderer.SetPosition(1, location);
            m_laserBeam.transform.position = location;
        }
    }

    public enum ELaserAttackType
    {
        Line,
        Follow,
        Circular
    };
}