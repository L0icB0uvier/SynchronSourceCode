using Effect;
using Lean.Pool;
using UnityEngine;

namespace Characters.DamageSystem
{
	public class Projectile : Damager, IPoolable
	{
		protected Rigidbody2D m_RB2D;

		protected GameObject m_Shooter;

		public GameObject explosionPrefab;

		protected AudioSource m_BulletAudioSource;

		public int lifeSpan = 3;

		protected float m_CurrentLifespan = 0;

		protected override void Awake()
		{
			base.Awake();
			m_RB2D = GetComponent<Rigidbody2D>();
			m_BulletAudioSource = GetComponent<AudioSource>();
		}

		protected void Update()
		{
			if (m_CurrentLifespan > 0)
			{
				m_CurrentLifespan = Mathf.Clamp(m_CurrentLifespan -= Time.deltaTime, 0, lifeSpan);
			}

			else if (gameObject.activeInHierarchy)
			{
				LeanPool.Despawn(gameObject);
			}
		}

		private void FixedUpdate()
		{
			CheckForContact();
		}

		public virtual void ShootProjectile(Vector2 shotFrom, Vector2 force, GameObject shooter)
		{
			transform.position = shotFrom;
			damageInstigator = shooter;
			m_RB2D.AddForce(force);
			m_CurrentLifespan = lifeSpan;
		}

		protected override void DamageableHit(Damageable damageable, Vector2 damageDirection)
		{
			base.DamageableHit(damageable, damageDirection);

			LeanPool.Spawn(explosionPrefab, transform.position, Quaternion.identity).GetComponent<ExplosionArea>();
			LeanPool.Despawn(gameObject);
		}

		protected override void NonDamageableHit()
		{
			LeanPool.Spawn(explosionPrefab, transform.position, Quaternion.identity).GetComponent<ExplosionArea>();
			LeanPool.Despawn(gameObject);
		}

		public virtual void OnSpawn()
		{
		
		}

		public virtual void OnDespawn()
		{
		
		}
	}
}
