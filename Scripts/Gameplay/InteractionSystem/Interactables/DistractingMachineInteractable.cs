using System;
using Gameplay.InteractionSystem.Interacters;
using Gameplay.PoweredObjects.ControlledPoweredObjects;
using GeneralScriptableObjects;
using UnityEngine;

namespace Gameplay.InteractionSystem.Interactables
{
    public class DistractingMachineInteractable : Interactable
    {
        [SerializeField] private DistractingMachine _distractingMachine;

        private void Awake()
        {
            _distractingMachine = transform.root.GetComponent<DistractingMachine>();
        }

        public override bool TryInteraction(Interacter interacter)
        {
            if (!IsInteractionPossible()) return false;

            if (_distractingMachine.MachineOn)
            {
                _distractingMachine.TurnMachineOff();
                HideActionText(interacter);
            }

            else
            {
                _distractingMachine.TurnMachineOn();
                ChangeActionText();
            }
            
            return true;
        }
        
        protected override bool IsInteractionPossible()
        {
            return !_distractingMachine.TurningOff;
        }

        protected override StringVariable GetActionText()
        {
            return _distractingMachine.AvailableAction;
        }
        
        public override void InteracterEntered(Interacter interacter)
        {
            base.InteracterEntered(interacter);
            _distractingMachine.onMachineTurnedOff.AddListener(MachineTurnedOff);
        }

        public override void InteracterExited(Interacter interacter)
        {
            base.InteracterExited(interacter);
            _distractingMachine.onMachineTurnedOff.RemoveListener(MachineTurnedOff);
        }
        
        private void MachineTurnedOff()
        {
            DisplayActionText(currentInteracter);
        }
    }
}