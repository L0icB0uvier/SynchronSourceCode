using GeneralScriptableObjects.Events;
using UnityEngine;
using UnityEngine.Events;

namespace UI.InputTutorial
{
    public class InputCatcher : MonoBehaviour
    {
        [SerializeField] private VoidEventChannelSO inputToCatch;

        [SerializeField] private UnityEvent onInputCaptured;
        
        private void OnEnable()
        {
            inputToCatch.onEventRaised += InputCaptured;
        }

        private void InputCaptured()
        {
            onInputCaptured?.Invoke();
        }

        private void OnDisable()
        {
            inputToCatch.onEventRaised -= InputCaptured;
        }
    }
}