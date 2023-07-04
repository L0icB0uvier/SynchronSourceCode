using UnityEngine;

namespace Characters.DamageSystem
{
	public class RadioactiveArea : Damager
	{
		public float damageRate = 1;

		float m_CurrentTime;

		protected override void NonDamageableHit()
		{
		
		}

		// Update is called once per frame
		void Update()
		{
			m_CurrentTime += Time.deltaTime;

			if(m_CurrentTime >+damageRate)
			{
				CheckForContact();
				m_CurrentTime = 0;
			}
		}
	}
}
