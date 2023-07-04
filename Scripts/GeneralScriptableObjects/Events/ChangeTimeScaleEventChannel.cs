using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/Time/ChangeTimeScale", order = 0)]
    public class ChangeTimeScaleEventChannel : ScriptableObject
    {
        public event UnityAction<float, bool, float> OnEventRaised;

        public void RaiseEvent(float desiredTimeScale, bool lerp, float lerpTime = 0)
        {
            OnEventRaised?.Invoke(desiredTimeScale, lerp, lerpTime);
        }
    }
}