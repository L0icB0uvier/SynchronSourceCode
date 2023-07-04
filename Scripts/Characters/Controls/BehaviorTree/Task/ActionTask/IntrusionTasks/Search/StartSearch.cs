using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies;
using SceneManagement.LevelManagement;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.IntrusionTasks.Search
{
	[TaskCategory("Behavior/Search")]
	public class StartSearch : Action
	{
		public SharedAIController AIController;

		EnemyAIController m_EnemyAIController;
	
		AreaManager m_Area;
	
		public override void OnAwake()
		{
			base.OnAwake();

			m_EnemyAIController = (EnemyAIController)AIController.Value;
			m_Area = m_EnemyAIController.CurrentArea;
		}

		public override TaskStatus OnUpdate()
		{
			if (!m_Area.SearchingArea)
			{
				m_Area.StartSearchInArea();
			}
		
			return TaskStatus.Success;
		}
	}
}