using UnityEngine;

namespace Utilities
{
    [CreateAssetMenu(fileName = "EnvironmentalQueriesData", menuName = "ScriptableObjects/Create Environmental Query data", order = 0)]
    public class EnvironmentalQueriesData : ScriptableObject
    {
        public LayerMask obstacleLayerMask;
        public LayerMask coverLayerMask;
        public LayerMask groundLayerMask;
    }
}