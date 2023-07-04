using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Jobs
{
	public class ExecutePowerSocketJob : Action
	{
		public SharedAIController AIController;

		private UnitAIController m_unitAIController;
	
		private PowerSocketJob m_powerSocketJob;
	
		public override void OnAwake()
		{
			base.OnAwake();

			m_unitAIController = (UnitAIController) AIController.Value;
		}

		public override void OnStart()
		{
			base.OnStart();
			m_powerSocketJob = (PowerSocketJob)m_unitAIController.JobsAssigned[0];
		}

		public override TaskStatus OnUpdate()
		{
			if(m_powerSocketJob.jobCompleted)
			{
				m_unitAIController.JobCompleted(m_powerSocketJob);
				return TaskStatus.Success;
			}

			return TaskStatus.Running;
		}

		public override void OnConditionalAbort()
		{
			base.OnConditionalAbort();
			m_unitAIController.CurrentArea.UnitReportJobInterruption(m_unitAIController);
		}
	}
}
