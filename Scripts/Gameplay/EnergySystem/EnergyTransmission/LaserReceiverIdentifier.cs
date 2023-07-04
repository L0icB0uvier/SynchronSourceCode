using SavingSystem;
using UnityEngine;

namespace Gameplay.EnergySystem.EnergyTransmission
{
    public class LaserReceiverIdentifier : MonoBehaviour
    {
        public ObjectUniqueIdentifier ObjectGUID { get; private set; }

        private void Awake()
        {
            ObjectGUID = GetComponent<ObjectUniqueIdentifier>();
        }

        private void Reset()
        {
            ObjectGUID = GetComponent<ObjectUniqueIdentifier>();
        }
    }
}