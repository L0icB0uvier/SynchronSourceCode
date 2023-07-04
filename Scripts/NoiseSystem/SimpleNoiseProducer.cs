using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using UnityEngine;

namespace NoiseSystem
{
    public class SimpleNoiseProducer : MonoBehaviour
    {
        [SerializeField] private NoiseEmissionProfile noiseEmissionProfile;
        [SerializeField] private GenerateNoiseChannel noiseChannel;

        public void GenerateNoise()
        {
            noiseChannel.RaiseEvent(transform.position, noiseEmissionProfile.noiseAmplitude, noiseEmissionProfile.stoppedByWalls, ENoiseInstigator.Player);
        }
    }
}