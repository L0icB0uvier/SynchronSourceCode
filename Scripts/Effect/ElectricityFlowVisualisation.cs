using System;
using Gameplay.EnergySystem.EnergyProduction;
using UnityEngine;

namespace Effect
{
	public class ElectricityFlowVisualisation : MonoBehaviour
	{
		[SerializeField] private EnergySource energySource;
		
		private void Awake()
		{
			energySource.onProducingPowerChanged += ChangeHologramState;
		}

		private void Start()
		{
			ChangeHologramState(energySource.ProducingPower);
		}

		private void OnDestroy()
		{
			energySource.onProducingPowerChanged -= ChangeHologramState;
		}

		private void ChangeHologramState(bool active)
		{
			gameObject.SetActive(active);
		}
	}
}
