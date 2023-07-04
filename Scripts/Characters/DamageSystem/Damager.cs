using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Characters.DamageSystem
{
	public abstract class Damager : MonoBehaviour
	{
		public GameObject damageInstigator;

		[Tooltip("If disabled, damager ignore trigger when casting for damage")]
		public bool disableDamageAfterHit;

		protected bool canDamage = true;

		protected Collider2D damagerCollider;

		[SerializeField]
		protected string[] damagedTags;
	
		[SerializeField]
		public ContactFilter2D m_AttackContactFilter;

		private readonly List<Collider2D> m_attackOverlapResults = new List<Collider2D>();
		
		protected virtual void Awake()
		{
			damagerCollider = GetComponent<Collider2D>();
		}

		public void EnableDamage()
		{
			canDamage = true;
		}

		public void DisableDamage()
		{
			canDamage = false;
		}

		public void CheckForContact()
		{
			if (!canDamage)
				return;

			int hitCount = damagerCollider.OverlapCollider(m_AttackContactFilter, m_attackOverlapResults);

			if(hitCount > 0)
			{
				Collider2D m_LastHit;

				for (int i = 0; i < hitCount; i++)
				{
					if (!damagedTags.Contains(m_attackOverlapResults[i].tag))
						return;
				
					m_LastHit = m_attackOverlapResults[i];

					//Make sure not to damage self
					if (m_LastHit.gameObject == damageInstigator)
					{
						continue;
					}

					Damageable damageable = m_LastHit.GetComponent<Damageable>();
				
					if (!damageable.CanBeDamaged)
					{
						return;
					}

					if (damageable)
					{
						Vector2 hitLocation = m_LastHit.ClosestPoint(transform.position);
						Vector2 pos2D = transform.position;
						Vector2 dir = (hitLocation - pos2D).normalized;

						DamageableHit(damageable, dir);
					}

					else
					{
						NonDamageableHit();
					}
				}
			}
		}

		protected void CheckForContact(Vector2 origin, float radius, Vector2 direction, float distance )
		{
			RaycastHit2D[] hits = Physics2D.CircleCastAll(origin, radius, direction, distance, m_AttackContactFilter.layerMask);

			if(hits.Length > 0)
			{
				foreach(RaycastHit2D hit in hits)
				{
					Damageable damageable = hit.collider.GetComponent<Damageable>();

					if (damageable)
					{
						DamageableHit(damageable, direction);
					}
				}
			}
		}

		protected virtual void DamageableHit(Damageable damageable, Vector2 damageDirection)
		{
			damageable.TakeDamage(this, damageDirection);

			if (disableDamageAfterHit)
				DisableDamage();
		}

		protected abstract void NonDamageableHit();

	}
}
