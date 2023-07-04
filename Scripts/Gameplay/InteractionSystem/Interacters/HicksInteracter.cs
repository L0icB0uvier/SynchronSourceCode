using System;
using GeneralScriptableObjects.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Gameplay.InteractionSystem.Interacters
{
    public class HicksInteracter : Interacter
    {
        [SerializeField] private VoidEventChannelSO interactChannel;
        
        protected override void Awake()
        {
            base.Awake();
            interactChannel.onEventRaised += TryInteraction;
        }

        private void OnDestroy()
        {
            interactChannel.onEventRaised -= TryInteraction;
        }

        public override void TryInteraction()
        {
            onInteractionTried?.Invoke(connectionInterface.TryInteraction());
        }
    }
}