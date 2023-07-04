using UnityEngine;
using UOP1.Factory;

namespace NoiseSystem
{
    [CreateAssetMenu(fileName = "New Noise factory", menuName = "Factory/Noise Factory")]
    public class NoiseProducerFactorySO : FactorySO<Noise>
    {
        public Noise prefab = default;
        
        public override Noise Create()
        {
            return Instantiate(prefab);
        }
    }
}