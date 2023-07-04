using System;
using System.Collections.Generic;
using Gameplay.InteractionSystem.Interactables;
using Gameplay.InteractionSystem.Switch;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Gameplay.InteractionSystem.Interacters
{
    public abstract class Interacter : MonoBehaviour
    {
        protected List<IInteractable> interactablesAvailable = new List<IInteractable>();
        public List<IInteractable> InteractablesAvailable => interactablesAvailable;
        
        [SerializeField] private InteracterProfileSO interacterProfile;
        public InteracterProfileSO InteracterProfile => interacterProfile;

        public ConnectionInterfaceManager ConnectionInterfaceManager { get; private set; }

        public abstract void TryInteraction();

        [FoldoutGroup("Events")] public UnityEvent <bool> onInteractionTried;

        [FormerlySerializedAs("connectedInterface")] [SerializeField] protected ConnectionInterface connectionInterface;

        protected virtual void Awake()
        {
            ConnectionInterfaceManager = transform.root.GetComponent<ConnectionInterfaceManager>();
        }

        public void OnInteractableEntered(IInteractable interactable)
        {
            if (interactablesAvailable.Contains(interactable)) return;
            interactablesAvailable.Add(interactable);
        }
        
        public void OnInteractableExited(IInteractable interactable)
        {
            if (!interactablesAvailable.Contains(interactable)) return;
            interactablesAvailable.Remove(interactable);
        }

        public void Initialize()
        {
            interactablesAvailable.Clear();
        }

        public virtual void DisableCharacterControl()
        {
            
        }

        public virtual void EnableCharacterControl()
        {
            
        }
    }
}