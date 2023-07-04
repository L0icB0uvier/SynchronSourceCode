using System;
using Characters.Controls.Input.Actions;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using InControl;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Controls.Input
{
    public class SkullfaceInputListener : MonoBehaviour, IInputListener
    {
        private SkullfaceActions m_skullfaceActions;
        
        [SerializeField] private VoidEventChannelSO[] enableInputChannels;
        [SerializeField] private VoidEventChannelSO[] disableInputsChannels;
        
        [SerializeField] private Vector2Variable movementInput;

        public bool ListenInputs { get; private set; }

        [SerializeField] private VoidEventChannelSO TeleportChannel;
        [SerializeField] private VoidEventChannelSO InteractChannel;
        [SerializeField] private VoidEventChannelSO BoostInputPressedChannel;
        [SerializeField] private VoidEventChannelSO BoostInputReleasedChannel;
        [SerializeField] private VoidEventChannelSO startFollowInputChannel;
        [SerializeField] private VoidEventChannelSO stopFollowInputChannel;
        
        [FoldoutGroup("InputStates")][SerializeField] private BoolVariable moveInputState;
        [FoldoutGroup("InputStates")][SerializeField] private BoolVariable teleportInputState;
        [FoldoutGroup("InputStates")][SerializeField] private BoolVariable interactInputState;
        [FoldoutGroup("InputStates")][SerializeField] private BoolVariable boostInputState;
        [FoldoutGroup("InputStates")][SerializeField] private BoolVariable followInputState;
        
        [SerializeField] private BoolVariableNotifyChange conversationActive;

        private void Awake()
        {
            m_skullfaceActions = new SkullfaceActions();
            AssignActionsBinding();

            ListenInputs = true;
        }

        private void OnEnable()
        {
	        foreach (var channel in enableInputChannels)
	        {
		        channel.onEventRaised += EnableInputs;
	        }
	        
	        foreach (var channel in disableInputsChannels)
	        {
		        channel.onEventRaised += DisableInputs;
	        }
        }

        private void OnDisable()
        {
	        foreach (var channel in enableInputChannels)
	        {
		        channel.onEventRaised -= EnableInputs;
	        }
	        
	        foreach (var channel in disableInputsChannels)
	        {
		        channel.onEventRaised -= DisableInputs;
	        }
        }

        public void AssignActionsBinding()
        {
            m_skullfaceActions.RSLeft.AddDefaultBinding(InputControlType.RightStickLeft);
            m_skullfaceActions.RSRight.AddDefaultBinding(InputControlType.RightStickRight);
            m_skullfaceActions.RSUp.AddDefaultBinding(InputControlType.RightStickUp);
            m_skullfaceActions.RSDown.AddDefaultBinding(InputControlType.RightStickDown);
            m_skullfaceActions.Boost.AddDefaultBinding(InputControlType.RightBumper);
            m_skullfaceActions.Teleport.AddDefaultBinding(InputControlType.RightTrigger);
            m_skullfaceActions.Interact.AddDefaultBinding(InputControlType.RightStickButton);
            m_skullfaceActions.Follow.AddDefaultBinding(InputControlType.LeftBumper);
        }
        
        private void Update()
        {
	        if (!ListenInputs || conversationActive.Value) return;
	        ReadInputs();
        }

        public void ReadInputs()
        {
	        
	        if(moveInputState.Value) movementInput.SetValue(m_skullfaceActions.RS.Value);
	        
	        if (m_skullfaceActions.Interact.WasPressed && interactInputState.Value)
	        {
		        InteractChannel.RaiseEvent();
	        }

	        if (m_skullfaceActions.Boost.WasPressed && boostInputState.Value)
	        {
		        BoostInputPressedChannel.RaiseEvent();
	        }

	        if (m_skullfaceActions.Boost.WasReleased && boostInputState.Value)
	        {
		        BoostInputReleasedChannel.RaiseEvent();
	        }
	        

	        if (m_skullfaceActions.Teleport.WasPressed && teleportInputState.Value)
	        {
		        TeleportChannel.RaiseEvent();
	        }

	        if (m_skullfaceActions.Follow.WasPressed && followInputState.Value)
	        {
		        startFollowInputChannel.RaiseEvent();
	        }
	        
	        if (m_skullfaceActions.Follow.WasReleased && followInputState.Value)
	        {
		        stopFollowInputChannel.RaiseEvent();
	        }
        }

        public void EnableInputs()
        {
	        ListenInputs = true;
	        movementInput.SetValue(Vector2.zero);
        }

        public void DisableInputs()
        {
	        ListenInputs = false;
	        movementInput.SetValue(Vector2.zero);
        }
    }
}