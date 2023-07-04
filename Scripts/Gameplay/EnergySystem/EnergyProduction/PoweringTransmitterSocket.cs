using Gameplay.EnergySystem.EnergyTransmission;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.EnergySystem.EnergyProduction
{
    public class PoweringTransmitterSocket : EnergySource, IReceiveLaser, ITransmitLaser
    {
        public Transform TransmitterTransform => transform;
        public Transform ReceiverTransform => transform;
        public bool IsReceivingLaser { get; private set; }

        public EnergyLaser OutgoingEnergyLaser { get; private set; }
        public LaserReceiverIdentifier TransmittingTo { get; }
        public UnityAction OnPoweredChanged { get; set; }

        public EnergyLaser IncomingEnergyLaser { get; private set; }

        [FoldoutGroup("Events")]
        public UnityEvent onReceiveLaser;
        
        [FoldoutGroup("Events")]
        public UnityEvent onLoseLaser;
        
        private void Start()
        {
            switch (IsReceivingLaser)
            {
                case true:
                    ReceiveLaser();
                    break;
                case false:
                    LoseLaser();
                    break;
            }
        }

        public void ReceiveLaser()
        {
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
        
        public void OutgoingLaserConnected(EnergyLaser energyLaser)
        {
            OutgoingEnergyLaser = energyLaser;
        }

        public void OutgoingLaserDisconnected()
        {
            OutgoingEnergyLaser = null;
        }

        public void TransmitterRemoved()
        {
            RemoveOutgoingLaser();
            RemoveIncomingLaser();
        }

        public void RemoveOutgoingLaser()
        {
            if(OutgoingEnergyLaser) OutgoingEnergyLaser.DespawnLaser();
        }
        

        public void RemoveIncomingLaser()
        {
            if(IncomingEnergyLaser) IncomingEnergyLaser.DespawnLaser();
        }
    }
}