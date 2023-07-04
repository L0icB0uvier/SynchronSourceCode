using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/Vector3 Event Channel")]
    public class Vector3EventChannelSO : ScriptableObject
    {
        public event UnityAction<Vector3> onEventRaised;

        public void RaiseEvent(Vector3 value)
        {
            onEventRaised?.Invoke(value);
        }
    }
}