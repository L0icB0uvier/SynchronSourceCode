using GeneralScriptableObjects.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace SavingSystem
{
    public class SaveEventListener : MonoBehaviour
    {
        [SerializeField] private VoidEventChannelSO saveEventChannel;
       
        public UnityEvent onSave;
        
        private void OnEnable()
        {
            saveEventChannel.onEventRaised += Save;
        }

        private void OnDisable()
        {
            saveEventChannel.onEventRaised -= Save;
        }

        private void Save() => onSave?.Invoke();
    }
}