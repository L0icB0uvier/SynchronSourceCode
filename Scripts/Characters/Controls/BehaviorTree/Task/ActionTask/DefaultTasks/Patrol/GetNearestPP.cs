using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.DefaultTasks.Patrol
{
	[TaskCategory("Behavior/Patrol")]
	public class GetNearestPP : Action
	{
		public SharedAIController AIController;

		private UnitAIController m_unitAIController;

		public SharedVector2 patrolPointLocation;
	
		public float minDistanceToNearestPatrolPoint;

		public override void OnAwake()
		{
			base.OnAwake();

			m_unitAIController = (UnitAIController)AIController.Value;
		}

		public override TaskStatus OnUpdate()	
		{
			if (!FindNearestPatrolPoint()) return TaskStatus.Failure;
		
			patrolPointLocation.Value = m_unitAIController.GetPathCurrentPatrolPoint().patrolPoint.transform.position;
			return TaskStatus.Success;
		}

		private bool FindNearestPatrolPoint()
		{
			var patrolPointFound = false;
			var nearestPatrolPointIndex = 0;
			float nearestPatrolPointDistance = 100000;

			if (m_unitAIController.CurrentPathPatrolPoints.Length == 0)
			{
				Debug.Log(m_unitAIController.name + " doesn't have patrol points");
				return false;
			}

			if(m_unitAIController.GetCurrentPathPatrolBehavior() == EPatrolBehavior.Fixe)
			{
				if (!m_unitAIController.GetCurrentPathPatrolPointAtIndex(0).patrolPoint)
				{
					Debug.Log(m_unitAIController.name + " Patrol point 0 is not assigned");
					return false;
				}
				m_unitAIController.CurrentPatrolPointIndex = 0;
				return true;
			}
			
			for (var i = 0; i < m_unitAIController.CurrentPathPatrolPoints.Length; i++)
			{
				if (!m_unitAIController.GetCurrentPathPatrolPointAtIndex(i).patrolPoint)
				{
					Debug.Log(m_unitAIController.transform.root.gameObject.name + " have missing patrol point");
					continue;
				}
			
				if(!m_unitAIController.GetCurrentPathPatrolPointAtIndex(i).accessible)
					continue;

				patrolPointFound = true;

				float distanceToPatrolPoint = (m_unitAIController.GetCurrentPathPatrolPointAtIndex(i).patrolPoint.transform.position - transform.position).sqrMagnitude;

				if (distanceToPatrolPoint < nearestPatrolPointDistance && distanceToPatrolPoint > minDistanceToNearestPatrolPoint * minDistanceToNearestPatrolPoint)
				{
					nearestPatrolPointDistance = distanceToPatrolPoint;
					nearestPatrolPointIndex = i;
				}
			}

			if (!patrolPointFound) return false;
		
			m_unitAIController.CurrentPatrolPointIndex = nearestPatrolPointIndex;
			return true;

		}
	}
}
