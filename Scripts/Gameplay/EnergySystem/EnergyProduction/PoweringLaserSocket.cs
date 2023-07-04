using Gameplay.EnergySystem.EnergyTransmission;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.EnergySystem.EnergyProduction
{
    public class PoweringLaserSocket : EnergySource, IReceiveLaser
    {
        public Transform ReceiverTransform => transform;
        public bool IsReceivingLaser { get; private set; }
        
        public UnityAction OnPoweredChanged { get; private set; }

        public EnergyLaser IncomingEnergyLaser { get; private set; }

        [FoldoutGroup("Events")]
        public UnityEvent onReceiveLaser;
        
        [FoldoutGroup("Events")]
        public UnityEvent onLoseLaser;
        
        public void ReceiveLaser()
        {
            if (IsReceivingLaser) return;
            IsReceivingLaser = true;
            onReceiveLaser?.Invoke();
            OnPoweredChanged?.Invoke();
            StartProducingPower();
        }

        public void LoseLaser()
        {
            IsReceivingLaser = false;
            onLoseLaser?.Invoke();
            OnPoweredChanged?.Invoke();
            StopProducingPower();
        }

        public void IncomingLaserConnected(EnergyLaser energyLaser)
        {
            IncomingEnergyLaser = energyLaser;
        }

        public void IncomingLaserDisconnected()
        {
            IncomingEnergyLaser = null;
        }

        public void TransmitterRemoved()
        {
            RemoveIncomingLaser();
        }
        
        public void RemoveIncomingLaser()
        {
            if(IncomingEnergyLaser) IncomingEnergyLaser.DespawnLaser();
        }
    }
}