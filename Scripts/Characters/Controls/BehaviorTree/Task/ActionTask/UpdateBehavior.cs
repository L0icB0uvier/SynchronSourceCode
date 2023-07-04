using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;

namespace Characters.Controls.BehaviorTree.Task.ActionTask
{
	[TaskCategory("Behavior/ChangeBehavior")]
	[TaskDescription("Simply reflect the state of the behavior tree to the AIController")]
	public class UpdateBehavior : Action
	{
		public SharedAIController AIController;
		private UnitAIController m_unitAIController;

		public EUnitBehavior newBehavior;

		public override void OnAwake()
		{
			base.OnAwake();
			m_unitAIController = (UnitAIController)AIController.Value;
		}
	
		public override TaskStatus OnUpdate()
		{
			m_unitAIController.ChangeCurrentBehavior(newBehavior);
			return TaskStatus.Success;
		}
	}
}