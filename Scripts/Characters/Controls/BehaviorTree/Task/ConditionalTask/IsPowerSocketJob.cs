using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask
{
	[TaskCategory("Job")]
	[TaskDescription("Check if the unit current job is powering socket")]
	public class IsPowerSocketJob : Conditional
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
			return m_unitAIController.JobsAssigned[0].JobType == UnitJob.EJobType.PowerSocket ? TaskStatus.Success : 
				TaskStatus
					.Failure;
		}
	}
}