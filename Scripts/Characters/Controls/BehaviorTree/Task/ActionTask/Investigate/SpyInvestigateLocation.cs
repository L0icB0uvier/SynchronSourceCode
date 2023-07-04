using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using UnityEngine;
using Utilities;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Investigate
{
	[TaskCategory("Behavior/Investigate")]
	[TaskDescription("Look at the given location, wait and return success")]
	public class SpyInvestigateLocation : Action
	{
		public SharedAIController AIController;

		public SharedVector2 locationToLookAt;

		Vector2 direction;

		Vector2 currentlocationToLookAt;

		float angleToLocation;

		public float smoothTime = .5f;

		public int rotationSpeed = 90;

		float velocity;

		public float waitTime = 2;
		float m_CurrentWaitTime;

		public override void OnStart()
		{
			m_CurrentWaitTime = 0;
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
			float newLookingAngle = Mathf.SmoothDampAngle(AIController.Value.LookingDirection, angleToLocation, ref velocity, smoothTime, rotationSpeed, Time.deltaTime);

			if (newLookingAngle < 0) newLookingAngle += 360;
			if (newLookingAngle >= 360) newLookingAngle = 0;

			AIController.Value.ChangeLookingDirection(newLookingAngle);

			if (MathCalculation.ApproximatelyEqualFloat(newLookingAngle, angleToLocation, 2))
			{
				m_CurrentWaitTime += Time.deltaTime;
				if(m_CurrentWaitTime >= waitTime)
				{
					m_CurrentWaitTime = 0;
					return TaskStatus.Success;
				}

				else
				{
					return TaskStatus.Running;
				}
			
			}
			
			else
				return TaskStatus.Running;

		}

		public override void OnConditionalAbort()
		{
			m_CurrentWaitTime = 0;
			base.OnConditionalAbort();
		}
	}
}