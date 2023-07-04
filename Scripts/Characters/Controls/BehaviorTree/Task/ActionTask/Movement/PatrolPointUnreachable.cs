using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Movement
{
	public class PatrolPointUnreachable : Action
	{
		public SharedAIController AIController;

		private UnitAIController m_unitAIController;

		public override void OnAwake()
		{
			base.OnAwake();
			m_unitAIController = (UnitAIController) AIController.Value;
		}

		public override TaskStatus OnUpdate()
		{
			m_unitAIController.SetCurrentPatrolPointUnreachable();
			return TaskStatus.Failure;
		}
	}
}