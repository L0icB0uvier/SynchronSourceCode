using UnityEngine;
using UOP1.Factory;
using UOP1.Pool;

namespace NoiseSystem
{
    [CreateAssetMenu(fileName = "NewNoisePool", menuName = "Pool/NoisePool", order = 0)]
    public class NoisePoolSO : ComponentPoolSO<Noise>
    {
        [SerializeField] private NoiseProducerFactorySO _factory;

        public override IFactory<Noise> Factory
        {
            get
            {
                return _factory;
            }
            set
            {
                _factory = value as NoiseProducerFactorySO;
            }
        }
    }
}