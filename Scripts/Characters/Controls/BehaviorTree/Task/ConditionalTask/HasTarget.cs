using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies;
using SceneManagement.LevelManagement;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask
{
	public class HasTarget : Conditional
	{
		public SharedAIController AIController;
		private EnemyAIController m_enemyAIController;
		private AreaManager m_area;

		public override void OnAwake()
		{
			base.OnAwake();
			m_enemyAIController = (EnemyAIController)AIController.Value;
			m_area = m_enemyAIController.CurrentArea;
		}

		public override TaskStatus OnUpdate()
		{
			return m_area.Targets.Count > 0 ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}