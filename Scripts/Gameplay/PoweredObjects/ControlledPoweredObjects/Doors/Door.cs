namespace Gameplay.PoweredObjects.ControlledPoweredObjects.Doors
{
	public class Door : PathBlockingMovingPoweredSystem
	{
		protected override void UpdateSystemColliders()
		{
			switch (ControlledElementState)
			{
				case EControlledElementState.DefaultState:
					foreach (var elementCollider in elementColliders)
					{
						elementCollider.enabled = true;
					}
					break;
            
				case EControlledElementState.AlteredState:
					foreach (var elementCollider in elementColliders)
					{
						elementCollider.enabled = false;
					}
					break;
			}
		
			ColliderChanged();
		}
	}
}