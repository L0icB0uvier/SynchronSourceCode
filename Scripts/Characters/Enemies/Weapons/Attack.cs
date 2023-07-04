using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Ennemies.Weapons
{
	public abstract class Attack : MonoBehaviour
	{
		[FoldoutGroup("Weapon Settings")]
		public float minRange;

		[FoldoutGroup("Weapon Settings")]
		public float maxRange;

		[FoldoutGroup("Debug")]
		[SerializeField]
		[ReadOnly]
		protected bool m_CanBeUsed = true;
		public bool CanBeUsed => m_CanBeUsed;

		[FoldoutGroup("Debug")]
		[SerializeField]
		[ReadOnly]
		private bool m_recharging = false;
		public bool Recharging => m_recharging;

		[FoldoutGroup("Weapon Settings")]
		public float rechargeTime;

		[FoldoutGroup("Debug")]
		[SerializeField]
		[ReadOnly]
		protected float m_CurrentRechargeTime;

		protected virtual void Update()
		{
			if (m_recharging)
			{
				m_CurrentRechargeTime = Mathf.Clamp(m_CurrentRechargeTime - Time.deltaTime, 0, rechargeTime);

				if (m_CurrentRechargeTime == 0)
				{
					m_recharging = false;
					m_CanBeUsed = true;
				}
				
				return;
			}
		}

		protected void StartWeaponCooldown()
		{
			m_CurrentRechargeTime = rechargeTime;
			m_recharging = true;
			m_CanBeUsed = false;
		}

	}
}
