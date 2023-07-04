using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/Vector2 Event Channel")]
    public class Vector2EventChannelSO : ScriptableObject
    {
        public event UnityAction<Vector2> OnEventRaised;

        public void RaiseEvent(Vector2 value)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(value);
        }
    }
}