using NoiseSystem;
using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(fileName = "ProduceNoiseChannel", menuName = "Events/Create Noise Channel", order = 0)]
    public class GenerateNoiseChannel : ScriptableObject
    {
        public event UnityAction<Vector2, float, bool, ENoiseInstigator> OnEventRaised;

        public void RaiseEvent(Vector2 noiseSource, float noiseAmplitude, bool stoppedByWalls, ENoiseInstigator noiseInstigator)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(noiseSource, noiseAmplitude, stoppedByWalls, noiseInstigator);
        }
    }
}