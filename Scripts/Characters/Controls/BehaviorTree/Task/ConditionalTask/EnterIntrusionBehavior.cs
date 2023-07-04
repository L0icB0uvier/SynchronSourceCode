using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies;
using SceneManagement.LevelManagement;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask
{
	[TaskCategory("Intrusion")]
	public class EnterIntrusionBehavior : Conditional
	{
		public SharedAIController AIController;

		EnemyAIController m_UnitAIController;

		AreaManager m_Area;

		public override void OnAwake()
		{
			base.OnAwake();

			m_UnitAIController = (EnemyAIController)AIController.Value;
			m_Area = m_UnitAIController.CurrentArea;
		}

		public override TaskStatus OnUpdate()
		{
			return m_Area.AreaStatus == EAreaStatus.Alert ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}
