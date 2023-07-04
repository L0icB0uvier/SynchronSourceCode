using Lean.Pool;
using UnityEngine;

namespace Effect
{
	public class DustEffect : MonoBehaviour, IPoolable
	{
		Animator m_DustAnimator;

		private void Awake()
		{
			m_DustAnimator = GetComponent<Animator>();
			DontDestroyOnLoad(this);
		}

		public void AnimationComplete()
		{
			LeanPool.Despawn(gameObject);
		}

		public void OnSpawn()
		{
			m_DustAnimator.Play("TPDust", -1, 0);
		}

		public void OnDespawn()
		{
		}
	}
}
