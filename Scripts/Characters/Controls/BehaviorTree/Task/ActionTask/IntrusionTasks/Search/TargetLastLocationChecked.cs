using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.BehaviorTree.SharedVariables;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies;
using SceneManagement.LevelManagement;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.IntrusionTasks.Search
{
	[TaskCategory("Behavior/Search")]
	public class TargetLastLocationChecked : Action
	{
		public SharedAIController AIController;

		EnemyAIController m_EnemyAIController;

		AreaManager m_Area;

		public SharedTargetInfo target;

		public override void OnAwake()
		{
			base.OnAwake();

			m_EnemyAIController = (EnemyAIController)AIController.Value;
			m_Area = m_EnemyAIController.CurrentArea;
		}

		public override TaskStatus OnUpdate()
		{
			target.Value.lastLocationChecked = true;
			return TaskStatus.Success;
		}
	}
}