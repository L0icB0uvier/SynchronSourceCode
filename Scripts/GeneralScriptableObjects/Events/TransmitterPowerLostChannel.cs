using Gameplay.EnergySystem.EnergyTransmission;
using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/LaserEvents/TransmitterLostPowerChannel", order = 0)]
    public class TransmitterPowerLostChannel : ScriptableObject
    {
        public UnityAction<ITransmitLaser> OnEventRaised;

        public void RaiseEvent(ITransmitLaser value)
        {
            OnEventRaised?.Invoke(value);
        }
    }
}