using Lean.Pool;
using UnityEngine;

namespace Effect
{
	public class ExplosionArea : MonoBehaviour, IPoolable
	{
		Animator m_ExplosionAnimator;

		AudioSource explosionSound;

		protected void Awake()
		{
			m_ExplosionAnimator = GetComponent<Animator>();
			explosionSound = GetComponent<AudioSource>();
		}

		public void OnExplosionFinished()
		{
			LeanPool.Despawn(gameObject);
		}

		public void OnSpawn()
		{
			m_ExplosionAnimator.Play("Explosion", -1, 0);
			explosionSound.Play();
		}

		public void OnDespawn()
		{
		
		}
	}
}
