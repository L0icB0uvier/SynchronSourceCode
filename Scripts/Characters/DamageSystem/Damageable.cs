using GeneralScriptableObjects.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Characters.DamageSystem
{
	public class Damageable : MonoBehaviour
	{
		public UnityEvent onReceivedMortalDamage;
		
		public bool CanBeDamaged { get; private set; } = true;

		[SerializeField] private UnityEvent onDied;
		
		public void TakeDamage(Damager damager, Vector2 damageDir)
		{
			DisableDamage();
			onDied?.Invoke();
		}

		[Button]
		public void DeathTest()
		{
			onDied?.Invoke();
		}

		public void EnableDamage()
		{
			CanBeDamaged = true;
		}

		public void DisableDamage()
		{
			CanBeDamaged = false;
		}

		public void ReceiveMortalDamage()
		{
			onReceivedMortalDamage?.Invoke();
		}
	}
}