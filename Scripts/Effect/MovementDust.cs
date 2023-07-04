using UnityEngine;

namespace Effect
{
    public class MovementDust : MonoBehaviour
    {
        private ParticleSystem dustParticleSystem;
        private ParticleSystem.EmissionModule m_dustEmissionModule;

        [SerializeField] private int defaultMinIntensity = 2;
        [SerializeField] private int defaultMaxIntensity = 4;
        

        private void Awake()
        {
            dustParticleSystem = GetComponent<ParticleSystem>();
            m_dustEmissionModule = dustParticleSystem.emission;
        }
        
        public void ChangeMovementDustIntensity(float multiplier)
        {
            m_dustEmissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(defaultMinIntensity * multiplier, 
            defaultMaxIntensity * multiplier);
        }
    }
}