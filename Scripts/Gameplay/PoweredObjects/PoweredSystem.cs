using Gameplay.EnergySystem.EnergyProduction;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.PoweredObjects
{
	public abstract class PoweredSystem : MonoBehaviour
	{
		[SerializeField][PropertyOrder(-10)]
		private EnergySource energySource;

		public EnergySource EnergySource => energySource;
		
		public bool Powered { get; private set; }

		[FoldoutGroup("Events")][PropertyOrder(5)]
		[FoldoutGroup("Events/PowerEvents")]
		public UnityEvent onGainPower;

		[FoldoutGroup("Events/PowerEvents")][PropertyOrder(5)]
		public UnityEvent onLosePower;

		public UnityAction<bool> onPowerStateChanged;

		protected virtual void OnEnable()
		{
			EnergySource.onProducingPowerChanged += UpdatePowerState;
		}

		protected virtual void OnDisable()
		{
			EnergySource.onProducingPowerChanged -= UpdatePowerState;
		}

		protected void UpdatePowerState(bool isPowered)
		{
			Powered = isPowered;
			onPowerStateChanged?.Invoke(Powered);

			if (isPowered)
			{
				OnGainPower();
				
			}

			else
			{
				OnLosePower();
			}
		}

		protected virtual void OnGainPower()
		{
			onGainPower?.Invoke();
		}

		protected virtual void OnLosePower()
		{
			onLosePower?.Invoke();
		}
	}
}
