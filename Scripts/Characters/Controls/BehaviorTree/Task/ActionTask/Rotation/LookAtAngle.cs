using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using UnityEngine;
using Utilities;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Rotation
{
	[TaskCategory("Movement/Rotation")]
	[TaskDescription("Rotate toward a direction")]
	public class LookAtAngle : Action
	{
		public SharedAIController AIController;

		public SharedInt angle;

		public bool facingTarget;

		public SharedInt rotationSpeed;

		public float smoothTime = .5f;

		public float angleTolerance = 2.5f;

		float velocity;

		public override TaskStatus OnUpdate()
		{
			float newLookingAngle = Mathf.SmoothDampAngle(AIController.Value.LookingDirection, angle.Value, ref velocity, smoothTime, rotationSpeed.Value, Time.deltaTime);

			if (newLookingAngle < 0) newLookingAngle += 360;
			if (newLookingAngle >= 360) newLookingAngle = 0;

			if (MathCalculation.ApproximatelyEqualFloat(newLookingAngle, 360, 1))
				newLookingAngle = 0;

			AIController.Value.ChangeLookingDirection(newLookingAngle);

			if (MathCalculation.ApproximatelyEqualFloat(newLookingAngle, angle.Value, 2) && !facingTarget)
				return TaskStatus.Success;

			else
				return TaskStatus.Running;	
		}
	}
}
