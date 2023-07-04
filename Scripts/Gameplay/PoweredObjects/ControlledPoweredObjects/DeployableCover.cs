namespace Gameplay.PoweredObjects.ControlledPoweredObjects
{
	public class DeployableCover : MovingPoweredSystem
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
