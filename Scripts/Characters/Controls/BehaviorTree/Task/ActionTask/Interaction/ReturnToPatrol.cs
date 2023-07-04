using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Interaction
{
	public class ReturnToPatrol : Action
	{
		public SharedAIController AIController;

		private UnitAIController m_unitAIController;

		public SharedVector2 MoveToLocation;

		public override void OnAwake()
		{
			base.OnAwake();
			m_unitAIController = (UnitAIController) AIController.Value;
		}

		public override TaskStatus OnUpdate()
		{
			MoveToLocation.Value = m_unitAIController.GetPathCurrentPatrolPoint().patrolPoint
				.transform.position;
			return TaskStatus.Success;
		}
	}
}