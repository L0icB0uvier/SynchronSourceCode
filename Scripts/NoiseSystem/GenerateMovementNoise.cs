using System.Collections;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using Lean.Pool;
using UnityEngine;
using Utilities;

namespace NoiseSystem
{
    public class GenerateMovementNoise : MonoBehaviour
    {
        [SerializeField] private BoolVariable isMoving;

        [SerializeField] private float maxSpeed;
        [SerializeField] private FloatVariable noiseEmissionInterval;
        
        private Rigidbody2D m_rb2d;

        private Coroutine m_producingNoise;

        [SerializeField] private NoiseEmissionProfile noiseEmissionProfile;
        
        private void Awake()
        {
            m_rb2d = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (isMoving.Value && m_producingNoise == null)
            {
                m_producingNoise = StartCoroutine(ProducingNoise());
            }

            if (!isMoving.Value && m_producingNoise != null)
            {
                StopCoroutine(m_producingNoise);
                m_producingNoise = null;
            }
        }

        private IEnumerator ProducingNoise()
        {
            while (true)
            {
                yield return new WaitForSeconds(noiseEmissionInterval.Value);
                var noiseGo = LeanPool.Spawn(PrefabInstantiationUtility.GetGameObjectRefByName("Noise"), transform.position, 
                Quaternion.identity);
                var noise = noiseGo.GetComponent<Noise>();
                noise.StartNoiseWave(noiseEmissionProfile.noiseAmplitude * GetSpeedFactor(), noiseEmissionProfile.stoppedByWalls, ENoiseInstigator.Player);
            }
        }

        private float GetSpeedFactor()
        {
            return Mathf.Clamp01(m_rb2d.velocity.sqrMagnitude / (maxSpeed * maxSpeed));
        }
    }
}