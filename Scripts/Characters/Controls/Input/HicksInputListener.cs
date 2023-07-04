using Characters.Controls.Input.Actions;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using InControl;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Controls.Input
{
    public class HicksInputListener : MonoBehaviour, IInputListener
    {
	    private HicksActions m_hicksActions;

	    [SerializeField] private VoidEventChannelSO[] enableInputChannels;
	    [SerializeField] private VoidEventChannelSO[] disableInputsChannels;
	    
	    [SerializeField] private Vector2Variable movementInput;
	 
	    [SerializeField] private VoidEventChannelSO teleportEventChannel;
	    [SerializeField] private VoidEventChannelSO interactEventChannel;
	    [SerializeField] private VoidEventChannelSO togglePauseMenuEventChannel;

	    [FoldoutGroup("InputStates")][SerializeField] private BoolVariable moveInputState;
	    [FoldoutGroup("InputStates")][SerializeField] private BoolVariable teleportInputState;
	    [FoldoutGroup("InputStates")][SerializeField] private BoolVariable interactInputState;

	    [SerializeField] private BoolVariableNotifyChange conversationActive;
	    
	    public bool ListenInputs { get; private set; }
	    
        public void AssignActionsBinding()
        {
	        m_hicksActions.LSLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
	        
            m_hicksActions.LSRight.AddDefaultBinding(InputControlType.LeftStickRight);
            
            m_hicksActions.LSDown.AddDefaultBinding(InputControlType.LeftStickDown);
            
            m_hicksActions.LSUp.AddDefaultBinding(InputControlType.LeftStickUp);

            m_hicksActions.Pause.AddDefaultBinding(InputControlType.Command);
            m_hicksActions.Pause.AddDefaultBinding(Key.Escape);

            m_hicksActions.StealthMode.AddDefaultBinding(InputControlType.LeftBumper);
            m_hicksActions.Teleport.AddDefaultBinding(InputControlType.LeftTrigger);

            m_hicksActions.Interact.AddDefaultBinding(InputControlType.LeftStickButton);
        }

        private void Awake()
        {
	        m_hicksActions = new HicksActions();
	        AssignActionsBinding();
	        movementInput.SetValue(Vector2.zero);
	        
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

        private void Update()
        {
	        if (!ListenInputs || conversationActive.Value) return;
	        ReadInputs();
        }

        public void ReadInputs()
        {
	        if(moveInputState.Value) movementInput.SetValue(m_hicksActions.LS.Value);
	        
	        if (m_hicksActions.Interact.WasPressed && interactInputState.Value)
            {
	            interactEventChannel.RaiseEvent();
	            return;
            }
	        
	        if (m_hicksActions.Teleport.WasPressed && teleportInputState.Value)
	        {
		        teleportEventChannel.RaiseEvent();
	        }

	        if (m_hicksActions.Pause.WasPressed)
	        {
		        togglePauseMenuEventChannel.RaiseEvent();
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
        }
    }
}