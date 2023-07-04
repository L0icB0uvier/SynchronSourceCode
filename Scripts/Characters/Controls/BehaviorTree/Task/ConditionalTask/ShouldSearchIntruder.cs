using System.Linq;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies;
using SceneManagement.LevelManagement;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask
{
	[TaskCategory("Search")]
	public class ShouldSearchIntruder : Conditional
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
			if (m_Area.Targets.Any(x => !x.lastLocationChecked))
			{
				return TaskStatus.Failure;
			}

			else
			{
				return TaskStatus.Success;
			}
		}
	}
}