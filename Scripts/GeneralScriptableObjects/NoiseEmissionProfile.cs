using NoiseSystem;
using UnityEngine;

namespace GeneralScriptableObjects
{
    [CreateAssetMenu(fileName = "NewNoiseEmissionProfile", menuName = "Gameplay/Noise Emission Profile", order = 0)]
    public class NoiseEmissionProfile : ScriptableObject
    {
        public float noiseAmplitude;
        public bool stoppedByWalls;
    }
}