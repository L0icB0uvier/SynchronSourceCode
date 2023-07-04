using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.PoweredObjects.ControlledPoweredObjects
{
    public abstract class PathBlockingMovingPoweredSystem : MovingPoweredSystem
    {
        [FoldoutGroup("References")]
        public Transform[] elementEntrances = new Transform[2];

        [FoldoutGroup("Settings")][PropertyOrder(0)]
        public bool mustBeClosedAfterPassing;

        [FoldoutGroup("Settings")][ShowIf("mustBeClosedAfterPassing")][Indent()][PropertyOrder(0)]
        public bool waitBeforeClosing;
    
        [FoldoutGroup("Settings")][ShowIf("ShowClosingDelay")][Indent()][PropertyOrder(0)]
        public float closingDelay = 1;

        private bool ShowClosingDelay()
        {
            return mustBeClosedAfterPassing && waitBeforeClosing;
        }
    }
}
