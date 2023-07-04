using GeneralScriptableObjects;
using Pathfinding;
using UnityEngine;

namespace Gameplay.PoweredObjects.ControlledPoweredObjects
{
    public class GraphUpdater : MonoBehaviour
    {
        private GraphUpdateObject guo;
        
        [SerializeField] private Bounds elementBounds;

        [SerializeField] private FloatReference graphUpdateDelay;

        private void Awake()
        {
            elementBounds.center = transform.position;
            guo = new GraphUpdateObject(elementBounds) {resetPenaltyOnPhysics = false};
        }

        public void UpdateGraph()
        {
            AstarPath.active.UpdateGraphs(guo, graphUpdateDelay);
        }

        public void UpdateGraph(Bounds bounds)
        {
            guo.bounds = bounds;
            AstarPath.active.UpdateGraphs(guo, graphUpdateDelay);
        }
    }
}