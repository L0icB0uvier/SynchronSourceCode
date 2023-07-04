using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;
using SceneManagement.NavigationPoints;
using UnityEngine;
using Utilities;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.DefaultTasks.Patrol
{
	[TaskCategory("Behavior/Patrol")]
	public class ExecutePatrolPointBehavior : Action
	{
		public SharedAIController AIController;
		private UnitAIController m_unitAIController;

		float currentWaitingTime;

		int lookAroundIndex;

		LookAroundInfo currentLookAround;

		float velocity;

		public float smoothTime = .5f;

		public override void OnAwake()
		{
			base.OnAwake();
			m_unitAIController = (UnitAIController) AIController.Value;
		}


		public override void OnStart()
		{
			currentWaitingTime = 0;

			if (m_unitAIController.GetPathCurrentPatrolPoint().patrolPoint.behaviorAtLocation.lookAround)
			{
				lookAroundIndex = 0;
				currentLookAround =m_unitAIController.GetPathCurrentPatrolPoint().patrolPoint.behaviorAtLocation.lookAroundAngles[lookAroundIndex];
			}	
		}

		public override TaskStatus OnUpdate()
		{
			//Look around
			if (m_unitAIController.GetPathCurrentPatrolPoint().patrolPoint.behaviorAtLocation.lookAround)
			{
				if(Mathf.Abs(Mathf.DeltaAngle(m_unitAIController.LookingDirection, currentLookAround.lookingAngle)) < 2)
				{
					if (currentLookAround.waitingTime > 0)
					{
						currentWaitingTime += Time.deltaTime;
						if (currentWaitingTime < currentLookAround.waitingTime)
						{
							return TaskStatus.Running;
						}

						else
						{
							currentWaitingTime = 0;
						}
					}

					if (lookAroundIndex < m_unitAIController.GetPathCurrentPatrolPoint().patrolPoint.behaviorAtLocation.lookAroundAngles.Count - 1)
					{
						lookAroundIndex++;
						currentLookAround = m_unitAIController.GetPathCurrentPatrolPoint().patrolPoint.behaviorAtLocation.lookAroundAngles[lookAroundIndex];
						return TaskStatus.Running;
					}

					else
					{
						return TaskStatus.Success;
					}
				}

				else
				{
					float newLookingAngle = Mathf.SmoothDampAngle(m_unitAIController.LookingDirection, currentLookAround.lookingAngle, ref velocity, smoothTime, currentLookAround.rotationSpeed, Time.deltaTime);

					if (newLookingAngle < 0)
					{
						newLookingAngle += 360;
					}

					if (newLookingAngle >= 360)
					{
						newLookingAngle -= 360;
					}

					if (MathCalculation.ApproximatelyEqualFloat(newLookingAngle, 360, 1))
						newLookingAngle = 0;

					m_unitAIController.ChangeLookingDirection(newLookingAngle);

					return TaskStatus.Running;
				}	
			}

			//Wait at location
			else
			{
				if (m_unitAIController.GetPathCurrentPatrolPoint().patrolPoint.behaviorAtLocation.waitingTime == 0)
					return TaskStatus.Success;

				currentWaitingTime += Time.deltaTime;
				if (currentWaitingTime >= m_unitAIController.GetPathCurrentPatrolPoint().patrolPoint.behaviorAtLocation.waitingTime)
					return TaskStatus.Success;

				else return TaskStatus.Running;
			}

		}
	}
}