using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.DefaultTasks.Patrol
{
	[TaskCategory("Behavior/Patrol")]
	public class GetNextPatrolPoint : Action
	{
		public SharedAIController AIController;
		private UnitAIController m_unitAIController;

		public SharedVector2 patrolPointLocation;

		public override void OnAwake()
		{
			base.OnAwake();
			m_unitAIController = (UnitAIController) AIController.Value;
		}

		public override void OnStart()
		{
			base.OnStart();
			m_unitAIController.OnJobCompleted?.Invoke();
		}

		public override TaskStatus OnUpdate()
		{
			for (int i = 0; i < m_unitAIController.CurrentPathPatrolPoints.Length - 1; i++)
			{
				GetPatrolPoint();
				if (m_unitAIController.GetPathCurrentPatrolPoint().accessible)
				{
					patrolPointLocation.Value = m_unitAIController.GetPathCurrentPatrolPoint().patrolPoint.transform.position;
					return TaskStatus.Success;
				}
			}
		
			m_unitAIController.IncrementPatrolPathIndex();
			return TaskStatus.Failure;
		}

		private void GetPatrolPoint()
		{
			switch (m_unitAIController.GetCurrentPathPatrolBehavior())
			{
				case EPatrolBehavior.Fixe:
					break;

				case EPatrolBehavior.Loop:
					if (m_unitAIController.CurrentPatrolPointIndex + 1 < m_unitAIController.CurrentPathPatrolPoints.Length)
					{
						m_unitAIController.CurrentPatrolPointIndex += 1;
					}

					else
					{
						m_unitAIController.CurrentPatrolPointIndex = 0;
					}
					break;

				case EPatrolBehavior.PingPong:
					if (m_unitAIController.PingPongDirection == 1 && m_unitAIController.CurrentPatrolPointIndex + 1 ==
						m_unitAIController.CurrentPathPatrolPoints.Length)
					{
						m_unitAIController.InversePingPongDirection();
					}

					if (m_unitAIController.PingPongDirection == -1 && m_unitAIController.CurrentPatrolPointIndex - 1 ==
						-1)
					{
						m_unitAIController.InversePingPongDirection();
					}
				
					m_unitAIController.CurrentPatrolPointIndex += m_unitAIController.PingPongDirection;
					break;
			
				case EPatrolBehavior.Random:
					m_unitAIController.CurrentPatrolPointIndex = Random.Range(0, m_unitAIController.CurrentPathPatrolPoints.Length - 1); 
					break;
			}
		}
	}
}
