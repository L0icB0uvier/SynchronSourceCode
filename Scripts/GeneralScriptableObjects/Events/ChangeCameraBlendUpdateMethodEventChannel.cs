using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/Camera/ChangeBlendUpdateMethod", order = 0)]
    public class ChangeCameraBlendUpdateMethodEventChannel : ScriptableObject
    {
        public event UnityAction<CinemachineBrain.BrainUpdateMethod> OnEventRaised;

        public void RaiseEvent(CinemachineBrain.BrainUpdateMethod blendUpdateMethod)
        {
            OnEventRaised?.Invoke(blendUpdateMethod);
        }
    }
}