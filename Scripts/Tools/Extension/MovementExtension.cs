using UnityEngine;

namespace Tools.Extension
{
	public class MovementExtension : MonoBehaviour
	{
		public static float GetVerticalSpeedModifier(float yMovement)
		{
			float verticalitySpeedModifier;

			if (yMovement < .5f)
				verticalitySpeedModifier = 1;
			else if (yMovement < .87)
				verticalitySpeedModifier = .9f;
			else
				verticalitySpeedModifier = .8f;

			return verticalitySpeedModifier;
		}
	}
}
