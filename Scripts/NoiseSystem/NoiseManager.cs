using System;
using GeneralScriptableObjects.Events;
using UnityEngine;

namespace NoiseSystem
{
    public class NoiseManager : MonoBehaviour
    {
        [SerializeField] private NoisePoolSO noisePool;

        [SerializeField] private int _poolInitialSize;

        [SerializeField] private GenerateNoiseChannel generateNoiseChannel;
        [SerializeField] private DespawnNoiseChannel despawnNoiseChannel;
        
       private void Awake()
        {
            noisePool.Prewarm(_poolInitialSize);
            noisePool.SetParent(this.transform);
        }

        private void OnEnable()
        {
            generateNoiseChannel.OnEventRaised += NoiseRequested;
            despawnNoiseChannel.OnEventRaised += DespawnNoise;
        }

        private void OnDisable()
        {
            generateNoiseChannel.OnEventRaised -= NoiseRequested;
        }
        
        private void NoiseRequested(Vector2 noiseSource, float noiseAmplitude, bool stoppedByWalls, ENoiseInstigator 
            instigator)
        {
            var noise = noisePool.Request();
            noise.transform.position = noiseSource;
            noise.StartNoiseWave(noiseAmplitude, stoppedByWalls, instigator);
        }
        
        private void DespawnNoise(Noise noise)
        {
            noisePool.Return(noise);
        }
    }
}