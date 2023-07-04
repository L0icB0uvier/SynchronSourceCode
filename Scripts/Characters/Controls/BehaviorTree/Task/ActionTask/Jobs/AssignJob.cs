using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;
using Characters.Enemies.Perception;
using Gameplay.PoweredObjects.ControlledPoweredObjects;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Jobs
{
	[TaskCategory("Job")]
	[TaskDescription("Assign Job to self")]
	public class AssignJob : Action
	{
		public SharedAIController AIController;

		private UnitAIController m_unitAIController;

		public SharedControlledElement ControlledElement;

		public override void OnAwake()
		{
			base.OnAwake();

			m_unitAIController = (UnitAIController)AIController.Value;
		}

		public override TaskStatus OnUpdate()
		{
			MovingPoweredSystem movingPoweredSystem = ControlledElement.Value;

			if (movingPoweredSystem.isMonitored && movingPoweredSystem.ControlledElementState != movingPoweredSystem.monitoredState)
			{
				if (movingPoweredSystem.Powered)
				{
					m_unitAIController.AssignJob(new MonitorControlledElementJob(movingPoweredSystem, movingPoweredSystem.monitoredState));
				}
			
				else if (!movingPoweredSystem.Powered)
				{
					m_unitAIController.AssignJob(new CheckSocketJob(movingPoweredSystem.EnergySource));
				}

				return TaskStatus.Success;
			}

			return TaskStatus.Failure;
		}
	}
}