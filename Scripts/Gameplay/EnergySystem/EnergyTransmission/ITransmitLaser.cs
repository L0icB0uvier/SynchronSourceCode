using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.EnergySystem.EnergyTransmission
{
    public interface ITransmitLaser
    {
        Transform TransmitterTransform { get; }
        bool IsReceivingLaser { get; }
        
        EnergyLaser OutgoingEnergyLaser { get; }

        LaserReceiverIdentifier TransmittingTo { get; }

        UnityAction OnPoweredChanged { get; set; }
        
        void OutgoingLaserConnected(EnergyLaser energyLaser);
        void OutgoingLaserDisconnected();

        void RemoveOutgoingLaser();
    }
}