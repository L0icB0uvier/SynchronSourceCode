using NoiseSystem;
using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(fileName = "DespawnNoiseChannel", menuName = "Events/Despawn Noise Channel", order = 0)]
    public class DespawnNoiseChannel : ScriptableObject
    {
        public event UnityAction<Noise> OnEventRaised;

        public void RaiseEvent(Noise noise)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(noise);
        }
    }
}