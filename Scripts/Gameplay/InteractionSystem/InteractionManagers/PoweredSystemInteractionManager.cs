using Gameplay.PoweredObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.InteractionSystem.InteractionManagers
{
    public class PoweredSystemInteractionManager : MonoBehaviour
    {
        private PoweredSystem m_poweredSystem;
    
        public UnityEvent onPoweredSystemGainPower;
        public UnityEvent onPoweredSystemLosePower;

        private void Awake()
        {
            m_poweredSystem = transform.root.GetComponent<PoweredSystem>();
        }

        private void OnEnable()
        {
            m_poweredSystem.onGainPower.AddListener(PoweredSystemGainPower);
            m_poweredSystem.onLosePower.AddListener(PoweredSystemLosePower);
        }

        private void OnDisable()
        {
            m_poweredSystem.onGainPower.RemoveListener(PoweredSystemGainPower);
            m_poweredSystem.onLosePower.RemoveListener(PoweredSystemLosePower);
        }

        private void PoweredSystemGainPower()
        {
            onPoweredSystemGainPower?.Invoke();
        }

        private void PoweredSystemLosePower()
        {
            onPoweredSystemLosePower?.Invoke();
        }
    }
}
