using Gameplay.EnergySystem.EnergyTransmission;
using Gameplay.InteractionSystem.Interacters;
using GeneralScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.InteractionSystem.Interactables
{
    public class SimpleTransmitterLaserInteractable : Interactable
    {
        private ITransmitLaser m_laserTransmitter;
        private IReceiveLaser m_energyReceiver;

        [SerializeField][FoldoutGroup("ActionUI/Action text")] private StringVariable _transmitLaser;
        [SerializeField][FoldoutGroup("ActionUI/Action text")] private StringVariable _retransmitLaser;
        [SerializeField][FoldoutGroup("ActionUI/Action text")] private StringVariable _connectLaserAction;
        
        [SerializeField] private LaserConnectionManager _laserConnectionManager;

        private bool m_isConnectingLaser;
        
        private void Awake()
        {
            var root = transform.root;
            m_laserTransmitter = root.GetComponentInChildren<ITransmitLaser>();
            m_energyReceiver = root.GetComponentInChildren<IReceiveLaser>();
        }

        protected override bool IsInteractionPossible()
        {
            switch (m_isConnectingLaser)
            {
                case true:
                    return _laserConnectionManager.IsConnectingLaser;
                case false:
                    return m_laserTransmitter.IsReceivingLaser && !_laserConnectionManager.IsConnectingLaser;
            }
        }

        public override void InteracterEntered(Interacter interacter)
        {
            base.InteracterEntered(interacter);
            
            if(!m_isConnectingLaser) return;
            
            _laserConnectionManager.EnterConnectInteractable(transform);
        }

        public override void InteracterExited(Interacter interacter)
        {
            base.InteracterExited(interacter);
            
            if(!m_isConnectingLaser) return;
            
            _laserConnectionManager.ExitConnectInteractable();
        }

        public override bool TryInteraction(Interacter interacter)
        {
            if (!IsInteractionPossible()) return false;

            switch (m_isConnectingLaser)
            {
                case true:
                    if (m_energyReceiver.IncomingEnergyLaser != null)
                    {
                        m_energyReceiver.IncomingEnergyLaser.DespawnLaser();
                    }
                    ClearActionUI();
                    _laserConnectionManager.ConnectLaser(m_energyReceiver, OnLaserConnected);
                    break;
                
                case false:
                    m_laserTransmitter.RemoveOutgoingLaser();
                    _laserConnectionManager.StartConnectingLaser(m_laserTransmitter, interacter.transform.root);
                    break;
            }
            
            return true;
        }
        
        protected override StringVariable GetActionText()
        {
            return m_isConnectingLaser switch
            {
                true => _connectLaserAction,
                false => m_laserTransmitter.OutgoingEnergyLaser != null ? _retransmitLaser : _transmitLaser
            };
        }

        private void OnLaserConnected()
        {
            _laserConnectionManager.ChangeConnectLaserUIOrigin(m_laserTransmitter);
        }
        
        public void ChangeInteractionTypeToConnectLaser()
        {
            m_isConnectingLaser = true;
        }
        
        public void ChangeInteractionTypeToTransmitLaser()
        {
            m_isConnectingLaser = false;
        }
    }
}