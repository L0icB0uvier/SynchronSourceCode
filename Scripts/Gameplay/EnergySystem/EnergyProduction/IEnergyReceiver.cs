using UnityEngine.Events;

namespace Gameplay.EnergySystem.EnergyProduction
{
    public interface IEnergyReceiver
    {
        public bool IsReceivingEnergy { get;}
        
        void LoseEnergy();
        
        void ReceiveEnergy();

        UnityAction OnEnergyReceived { get; set; }
        UnityAction OnEnergyLost { get; set; }
    }
}