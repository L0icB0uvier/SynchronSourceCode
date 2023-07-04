using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using UnityEngine;
using Utilities;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Rotation
{
	[TaskCategory("Movement/Rotation")]
	[TaskDescription("Look at the given game object.")]
	public class LookAtGameObject : Action
	{
		public SharedAIController AIController;

		public SharedGameObject Target;

		public bool updateRotation;
	
		float angleToLocation;

		public float smoothTime = .5f;

		public int maxRotationSpeed = 90;

		private float m_velocity;

		public bool returnSuccess;

		public override void OnStart()
		{
			FindLookAtLocation();
		}

		private void FindLookAtLocation()
		{
			Vector2 direction = (Target.Value.transform.position - AIController.Value.transform.position).normalized;
			angleToLocation = Mathf.RoundToInt(MathCalculation.ConvertDirectionToAngle(direction));
		}

		public override TaskStatus OnUpdate()
		{
			if (updateRotation) FindLookAtLocation();

			float newLookingAngle = Mathf.SmoothDampAngle(AIController.Value.LookingDirection, angleToLocation, ref m_velocity, smoothTime, maxRotationSpeed, Time.deltaTime);

			AIController.Value.ChangeLookingDirection(newLookingAngle);

			if (returnSuccess && Mathf.Abs(Mathf.DeltaAngle(newLookingAngle, angleToLocation))< 2)  return TaskStatus.Success;
		
			return TaskStatus.Running;
		}
	}
}