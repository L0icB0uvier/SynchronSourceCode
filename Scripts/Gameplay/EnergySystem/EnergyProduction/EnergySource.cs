using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.EnergySystem.EnergyProduction
{
    public class EnergySource : MonoBehaviour
    {
        public enum EEnergySourceType
        {
            PowerSocket,
            LaserReceiver
        }

        public EEnergySourceType energySourceType;

        public bool ProducingPower { get; private set; }
        
        public Action<bool> onProducingPowerChanged;
        
        [FoldoutGroup("Events")]
        public UnityEvent onStartProducingPower;
        
        [FoldoutGroup("Events")]
        public UnityEvent onStopProducingPower;

        protected void StartProducingPower()
        {
            ProducingPower = true;
            onProducingPowerChanged?.Invoke(true);
            onStartProducingPower?.Invoke();
        }

        protected void StopProducingPower()
        {
            ProducingPower = false;
            onProducingPowerChanged?.Invoke(false);
            onStopProducingPower?.Invoke();
        }
    }
}
