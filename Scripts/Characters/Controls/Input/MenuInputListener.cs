using System;
using GeneralScriptableObjects.Events;
using InControl;
using UnityEngine;
using UnityEngine.Events;

namespace Characters.Controls.Input
{
    public class MenuInputListener : MonoBehaviour, IInputListener
    {
        private MenuUIActions m_menuUIActions;

        private bool m_listenInputs = true;
        public bool ListenInputs => m_listenInputs;
        
        [SerializeField] private VoidEventChannelSO[] enableInputsChannels;
        [SerializeField] private VoidEventChannelSO[] disableInputsChannels;

        public UnityEvent onPauseButtonPress;
        
        private void Awake()
        {
            m_menuUIActions = new MenuUIActions();
            AssignActionsBinding();
        }

        private void OnEnable()
        {
            foreach (var channel in enableInputsChannels)
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
            foreach (var channel in enableInputsChannels)
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
            m_menuUIActions.Pause.AddDefaultBinding(InputControlType.Command);
        }

        private void Update()
        {
            if (!m_listenInputs) return;
            ReadInputs();
        }

        public void ReadInputs()
        {
            if (m_menuUIActions.Pause.WasPressed)
            {
                onPauseButtonPress?.Invoke();
            }
        }

        public void EnableInputs()
        {
            m_listenInputs = true;
        }

        public void DisableInputs()
        {
            m_listenInputs = false;
        }
    }
}
