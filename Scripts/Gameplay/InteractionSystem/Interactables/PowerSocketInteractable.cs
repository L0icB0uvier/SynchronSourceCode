using System;
using Gameplay.EnergySystem.EnergyProduction;
using Gameplay.InteractionSystem.Interacters;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.InteractionSystem.Interactables
{
    public class PowerSocketInteractable : Interactable
    {
        [SerializeField] private EnergySocket socket;
        [SerializeField][FoldoutGroup("ActionUI/Action text")] private StringVariable powerSocket;
        [SerializeField][FoldoutGroup("ActionUI/Action text")] private StringVariable unpowerSocket;

        [FoldoutGroup("Events")] public UnityEvent onPowerInteraction;
        [FoldoutGroup("Events")] public UnityEvent onUnpowerInteraction;

        public GameObjectEventChannelSO powerSocketEventChannel;
        public GameObjectEventChannelSO unpowerSocketEventChannel;
        
        protected override bool IsInteractionPossible()
        {
            return socket.socketState != ESocketState.PoweredByEnergyCell;
        }

        public override bool TryInteraction(Interacter interacter)
        {
            if (!IsInteractionPossible()) return false;
            
            switch (socket.socketState)
            {
                case ESocketState.Unpowered:
                    onPowerInteraction?.Invoke();
                    interacter.DisableCharacterControl();
                    powerSocketEventChannel.RaiseEvent(interacter.transform.root.gameObject);
                    ChangeActionText();
                    break;
                case ESocketState.PoweredByEnergyCell:
                    break;
                case ESocketState.PoweredByRobot:
                    onUnpowerInteraction?.Invoke();
                    interacter.EnableCharacterControl();
                    unpowerSocketEventChannel.RaiseEvent(interacter.transform.root.gameObject);
                    ChangeActionText();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }
        
        protected override StringVariable GetActionText()
        {
            switch (socket.socketState)
            {
                case ESocketState.Unpowered:
                    return socket.RobotMovingToSocket ? unpowerSocket : powerSocket;
                case ESocketState.PoweredByEnergyCell:
                    return powerSocket;
                case ESocketState.PoweredByRobot:
                    return unpowerSocket;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}