using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Rotation
{
	[TaskCategory("Movement/Rotation")]
	public class LookAround : Action
	{
		public SharedAIController AIController;

		public int rotationSpeed;

		public float smoothTime = .5f;

		public float angleTolerance = 2f;

		private float m_velocity;

		public int lookAroundAngle = 40;

		private float m_targetAngle;

		private int m_currentAngleIndex;

		public bool returnToInitialRotation;

		private float m_initialAngle;

		public override void OnStart()
		{
			m_initialAngle = AIController.Value.LookingDirection;
			m_targetAngle = Mathf.LerpAngle(AIController.Value.LookingDirection, AIController.Value.LookingDirection + lookAroundAngle, 1);
			m_currentAngleIndex = 0;
		}

		public override TaskStatus OnUpdate()
		{
			float newLookingAngle = Mathf.SmoothDampAngle(AIController.Value.LookingDirection, m_targetAngle, ref m_velocity, smoothTime, rotationSpeed, Time.deltaTime);

			AIController.Value.ChangeLookingDirection(newLookingAngle);

			if (Mathf.Abs(Mathf.DeltaAngle(newLookingAngle, m_targetAngle)) < angleTolerance)
			{
				if (returnToInitialRotation)
				{
					if (m_currentAngleIndex == 0)
					{
						m_targetAngle = Mathf.LerpAngle(AIController.Value.LookingDirection, AIController.Value.LookingDirection - (lookAroundAngle * 2), 1);
						m_currentAngleIndex++;
						return TaskStatus.Running;
					}

					if (m_currentAngleIndex == 1)
					{
						m_targetAngle = Mathf.LerpAngle(AIController.Value.LookingDirection, m_initialAngle, 1);
						m_currentAngleIndex++;
						return TaskStatus.Running;
					}

					if (m_currentAngleIndex == 2)
					{
						return TaskStatus.Success;
					}
				}

				else
				{
					if(m_currentAngleIndex == 1) return TaskStatus.Success;
					m_targetAngle = Mathf.LerpAngle(AIController.Value.LookingDirection, AIController.Value.LookingDirection - (lookAroundAngle * 2), 1);

					m_currentAngleIndex++;
				}
			}
		
			return TaskStatus.Running;
		}
	}
}