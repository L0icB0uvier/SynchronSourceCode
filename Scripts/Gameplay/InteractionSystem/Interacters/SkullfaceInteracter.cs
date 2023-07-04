using Gameplay.EnergySystem.EnergyTransmission;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.InteractionSystem.Interacters
{
    public class SkullfaceInteracter : Interacter
    {
        [SerializeField] private LaserConnectionManager _laserConnectionManager;
        
        [SerializeField] private VoidEventChannelSO interactChannel;

        [SerializeField] private BoolVariableNotifyChange movementDisabledByInteracter;
        
        protected override void Awake()
        {
            base.Awake();
            interactChannel.onEventRaised += TryInteraction;
        }

        private void OnDestroy()
        {
            interactChannel.onEventRaised -= TryInteraction;
            movementDisabledByInteracter.SetValue(false);
        }

        public override void TryInteraction()
        {
            if (connectionInterface.TryInteraction())
            {
                onInteractionTried?.Invoke(true);
            }

            else
            {
                if (_laserConnectionManager.IsConnectingLaser)
                {
                    _laserConnectionManager.StopConnection();
                }

                else
                {
                    onInteractionTried?.Invoke(false);
                }
            }
        }

        public override void EnableCharacterControl()
        {
            base.EnableCharacterControl();
            movementDisabledByInteracter.SetValue(false);
        }

        public override void DisableCharacterControl()
        {
            base.DisableCharacterControl();
            movementDisabledByInteracter.SetValue(true);
        }
    }
}