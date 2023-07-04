using Gameplay.EnergySystem.EnergyTransmission;
using Gameplay.InteractionSystem.Interacters;
using GeneralScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.InteractionSystem.Interactables
{
    public class LaserReceptionInteractable : Interactable
    {
        private IReceiveLaser m_energyReceiver;

        [SerializeField][FoldoutGroup("ActionUI/Action text")] private StringVariable _connectLaserAction;
        
        [SerializeField] private LaserConnectionManager _laserConnectionManager;
        
        private void Awake()
        {
            m_energyReceiver = transform.root.gameObject.GetComponentInChildren<IReceiveLaser>();
        }
        
        public override void InteracterEntered(Interacter interacter)
        {
            base.InteracterEntered(interacter);

            _laserConnectionManager.EnterConnectInteractable(transform);
        }

        public override void InteracterExited(Interacter interacter)
        {
            base.InteracterExited(interacter);
            
            _laserConnectionManager.ExitConnectInteractable();
        }
        
        protected override bool IsInteractionPossible()
        {
            return _laserConnectionManager.IsConnectingLaser;
        }

        public override bool TryInteraction(Interacter interacter)
        {
            if (!IsInteractionPossible()) return false;

            if (m_energyReceiver.IncomingEnergyLaser != null)
            {
                m_energyReceiver.IncomingEnergyLaser.DespawnLaser();
            }

            _laserConnectionManager.ConnectLaser(m_energyReceiver, OnLaserConnected);
            
            return true;
        }

        private void OnLaserConnected()
        {
            _laserConnectionManager.StopConnection();
        }

        protected override StringVariable GetActionText()
        {
            return _connectLaserAction;
        }
    }
}