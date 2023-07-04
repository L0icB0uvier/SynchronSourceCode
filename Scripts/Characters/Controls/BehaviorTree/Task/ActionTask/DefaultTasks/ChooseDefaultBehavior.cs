using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.DefaultTasks
{
	[TaskCategory("Behavior")]
	public class ChooseDefaultBehavior : BehaviorTreeReference
	{
		public SharedAIController AIController;
		EnemyAIController m_UnitAIController;

		public override ExternalBehavior[] GetExternalBehaviors()
		{
			ExternalBehavior[] behaviorToLoad = new ExternalBehavior[1];
			m_UnitAIController = (EnemyAIController)AIController.Value;

			if (m_UnitAIController.defaultBehavior == EEnemyDefaultBehavior.Default)
			{
				behaviorToLoad[0] = externalBehaviors[0];
			}

			else
			{
				behaviorToLoad[0] = externalBehaviors[1];
			}

			return behaviorToLoad;
		}
	}
}