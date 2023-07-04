using System;

namespace Gameplay.PoweredObjects.ControlledPoweredObjects
{
    [Serializable]
    public class BridgeController : PathBlockingMovingPoweredSystem
    {
        protected override void UpdateSystemColliders()
        {
            switch (ControlledElementState)
            {
                case EControlledElementState.DefaultState:
                    foreach (var elementCollider in elementColliders)
                    {
                        elementCollider.enabled = false;
                    }
                    break;
            
                case EControlledElementState.AlteredState:
                    foreach (var elementCollider in elementColliders)
                    {
                        elementCollider.enabled = true;
                    }
                    break;
            }

            ColliderChanged();
        }
    }
}
