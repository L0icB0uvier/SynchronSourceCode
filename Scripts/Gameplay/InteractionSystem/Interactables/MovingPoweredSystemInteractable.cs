using Gameplay.InteractionSystem.Interacters;
using Gameplay.PoweredObjects.ControlledPoweredObjects;
using GeneralScriptableObjects;
using UnityEngine;

namespace Gameplay.InteractionSystem.Interactables
{
    public class MovingPoweredSystemInteractable : Interactable
    {
        [SerializeField] private MovingPoweredSystem _movingPoweredSystem;

        private void Awake()
        {
            _movingPoweredSystem = transform.root.GetComponent<MovingPoweredSystem>();
        }

        protected override bool IsInteractionPossible()
        {
            if (currentInteracter == null) return false;
            return !_movingPoweredSystem.IsTransitioning;
        }

        public override bool TryInteraction(Interacter interacter)
        {
            if (!IsInteractionPossible()) return false;
            
            _movingPoweredSystem.SwitchSystemState();
            HideActionText(interacter);
            return true;
        }

        protected override StringVariable GetActionText()
        {
            return _movingPoweredSystem.AvailableAction;
        }

        private void OnTransitionOver()
        {
            if (IsInteractionPossible())
            {
                DisplayActionText(currentInteracter);
            }
        }

        public override void InteracterEntered(Interacter interacter)
        {
            base.InteracterEntered(interacter);
            _movingPoweredSystem.onTransitionOver += OnTransitionOver;
            
            if (interacter.InteracterProfile.interacterActionType == InteracterProfileSO.EInteracterActionType
                .ShowNoActionText) return;
            _movingPoweredSystem.OnInteracterEnter();
        }

        public override void InteracterExited(Interacter interacter)
        {
            base.InteracterExited(interacter);
            _movingPoweredSystem.onTransitionOver -= OnTransitionOver;
            
            if (interacter.InteracterProfile.interacterActionType == InteracterProfileSO.EInteracterActionType
                .ShowNoActionText) return;
            _movingPoweredSystem.OnInteracterExit();
        }
    }
}