using UnityEngine;
using BehaviorDesigner.Runtime;
using Gameplay.EnergySystem.EnergyProduction;

[System.Serializable]
public class SharedEnergySocket : SharedVariable<EnergySocket>
{
	public override string ToString() { return mValue == null ? "null" : mValue.ToString(); }
	public static implicit operator SharedEnergySocket(EnergySocket value) { return new SharedEnergySocket { mValue = value }; }
}