using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using UnityEngine;
using Utilities;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Rotation
{
	[TaskCategory("Movement/Rotation")]
	[TaskDescription("Look at the given position.")]
	public class LookAtLocation : Action
	{
		public SharedAIController AIController;

		public SharedVector2 locationToLookAt;

		public bool updateRotation;

		Vector2 direction;

		Vector2 currentlocationToLookAt;

		float angleToLocation;

		public float smoothTime = .5f;

		public int rotationSpeed = 90;

		float velocity;

		public override void OnStart()
		{
			FindLookAtLocation();
		}

		private void FindLookAtLocation()
		{
			currentlocationToLookAt = locationToLookAt.Value;
			Vector2 position2D = transform.position;
			Vector2 direction = (currentlocationToLookAt - position2D).normalized;
			angleToLocation = Mathf.RoundToInt(MathCalculation.ConvertDirectionToAngle(direction));
		}

		public override TaskStatus OnUpdate()
		{
			if (updateRotation)
				FindLookAtLocation();

			float newLookingAngle = Mathf.SmoothDampAngle(AIController.Value.LookingDirection, angleToLocation, ref velocity, smoothTime, rotationSpeed, Time.deltaTime);

			if (newLookingAngle < 0) newLookingAngle += 360;
			if (newLookingAngle >= 360) newLookingAngle = 0;

			AIController.Value.ChangeLookingDirection(newLookingAngle);

			if (MathCalculation.AreAngleApproximatelyEqual(newLookingAngle, angleToLocation, 2) && !updateRotation) return TaskStatus.Success;
			
			return TaskStatus.Running;
		}
	}
}