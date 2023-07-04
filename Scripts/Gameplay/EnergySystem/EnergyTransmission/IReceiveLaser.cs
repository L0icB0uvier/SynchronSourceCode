using UnityEngine;

namespace Gameplay.EnergySystem.EnergyTransmission
{
    public interface IReceiveLaser
    {
        Transform ReceiverTransform { get; }
        bool IsReceivingLaser { get; }
        EnergyLaser IncomingEnergyLaser { get; }
        
        void ReceiveLaser();
        void LoseLaser();
        void IncomingLaserConnected(EnergyLaser energyLaser);
        void IncomingLaserDisconnected();
        
        void RemoveIncomingLaser();
    }
}