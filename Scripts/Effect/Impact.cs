using Lean.Pool;
using UnityEngine;

namespace Effect
{
	public class Impact : MonoBehaviour, IPoolable
	{
		Animator m_Animator;

		private void Awake()
		{
			m_Animator = GetComponent<Animator>();
		}

		public void OnPlayImpact(float impactDirection)
		{
			m_Animator.SetFloat("ImpactDirection", impactDirection);
		}

		public void OnAnimationComplete()
		{
			LeanPool.Despawn(gameObject);
		}

		public void OnSpawn()
		{
			m_Animator.Play("DashImpact", -1, 0);
		}

		public void OnDespawn()
		{
		
		}
	}
}
