using Gameplay.EnergySystem.EnergyTransmission;
using Gameplay.InteractionSystem.Interacters;
using GeneralScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.InteractionSystem.Interactables
{
    public class LaserTransmissionInteractable : Interactable
    {
        private ITransmitLaser m_laserTransmitter;

        [SerializeField][FoldoutGroup("ActionUI/Action text")] private StringVariable _transmitLaser;
        [SerializeField][FoldoutGroup("ActionUI/Action text")] private StringVariable _retransmitLaser;
        
        [SerializeField] private LaserConnectionManager _laserConnectionManager;
        
        private void Awake()
        {
            m_laserTransmitter = transform.root.GetComponentInChildren<ITransmitLaser>();
        }

        protected override bool IsInteractionPossible()
        {
            return m_laserTransmitter.IsReceivingLaser && !_laserConnectionManager.IsConnectingLaser;
        }

        public override bool TryInteraction(Interacter interacter)
        {
            if (!IsInteractionPossible()) return false;
            ForbidInteraction();
            m_laserTransmitter.RemoveOutgoingLaser();
            _laserConnectionManager.StartConnectingLaser(m_laserTransmitter, interacter.transform.root);
            return true;
        }

        protected override StringVariable GetActionText()
        {
            return m_laserTransmitter.OutgoingEnergyLaser != null ? _retransmitLaser : _transmitLaser;
        }
    }
}
