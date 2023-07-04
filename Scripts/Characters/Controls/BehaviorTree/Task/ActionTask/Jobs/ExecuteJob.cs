using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;
using Gameplay.EnergySystem.EnergyProduction;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Jobs
{
	[TaskCategory("Job")]
	[TaskDescription("Execute Job")]
	public class ExecuteJob : Action
	{
		public SharedAIController AIController;

		private UnitAIController m_unitAIController;

		private UnitJob m_job;
		
		public override void OnAwake()
		{
			base.OnAwake();

			m_unitAIController = (UnitAIController) AIController.Value;
		}

		public override void OnStart()
		{
			base.OnStart();
			if (m_unitAIController.JobsAssigned.Count == 0) return;
			m_job = m_unitAIController.JobsAssigned[0];
		}

		public override TaskStatus OnUpdate()
		{
			if (m_job == null) return TaskStatus.Failure;
			
			switch (m_job.JobType)
			{
				case UnitJob.EJobType.CheckSocket:
					m_unitAIController.JobCompleted(m_job);
					return TaskStatus.Success;

				case UnitJob.EJobType.PowerSocket:
					var powerSocketJob = (PowerSocketJob)m_job;
					
					if(!powerSocketJob.Socket.RobotMovingToSocket && powerSocketJob.Socket.socketState == ESocketState.Unpowered) m_unitAIController.Interact();
				
					if(powerSocketJob.jobCompleted)
					{
						m_unitAIController.JobCompleted(m_job);
						return TaskStatus.Success;
					}
					break;
			
				case UnitJob.EJobType.ModifyControlledElementState:
					var changeControlledElementStateJob = (MonitorControlledElementJob)m_job;

					if (!changeControlledElementStateJob.MovingPoweredSystem.Powered)
					{
						m_unitAIController.JobCompleted(m_job);
						m_unitAIController.AssignJob(new CheckSocketJob((EnergySocket)changeControlledElementStateJob.MovingPoweredSystem.EnergySource));
						return TaskStatus.Failure;
					}
				
					if (changeControlledElementStateJob.MovingPoweredSystem.IsTransitioning)
					{
						return TaskStatus.Running;
					}

					if (changeControlledElementStateJob.MovingPoweredSystem.ControlledElementState !=
					    changeControlledElementStateJob.DesiredState)
					{
						m_unitAIController.Interact();
					}
				
					switch (changeControlledElementStateJob.endJobOn)
					{
						case UnitJob.EJobCompleteCondition.Event:
							if (changeControlledElementStateJob.jobCompleted)
							{
								m_unitAIController.JobCompleted(m_job);
								return TaskStatus.Success;
							}
							break;
						case UnitJob.EJobCompleteCondition.JobCompleted:
							if (changeControlledElementStateJob.MovingPoweredSystem.ControlledElementState == changeControlledElementStateJob
								.DesiredState)
							{
								m_unitAIController.JobCompleted(m_job);
								return TaskStatus.Success;
							}
							break;
					}
					break;
				
				case UnitJob.EJobType.TurnOnDistractingMachine:
					var turnOffMachineJob = (TurnOnDistractingMachineJob)m_job;

					if (turnOffMachineJob.DistractingMachine.MachineOn) return TaskStatus.Success;
					
					m_unitAIController.Interact();
					m_unitAIController.JobCompleted(m_job);
					
					break;
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