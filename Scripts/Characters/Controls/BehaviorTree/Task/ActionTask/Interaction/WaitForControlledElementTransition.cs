using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units.ControllerImplementation;
using Gameplay.PoweredObjects.ControlledPoweredObjects;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Interaction
{
	public class WaitForControlledElementTransition : Action
	{
		public SharedAIController AIController;

		private SentryAIController m_sentryAIController;

		public SharedControlledElement MonitoredControlledElement;

		public override void OnAwake()
		{
			base.OnAwake();
			m_sentryAIController = (SentryAIController) AIController.Value;
		}

		public override TaskStatus OnUpdate()
		{
			if (MonitoredControlledElement.Value.IsTransitioning)
			{
				return TaskStatus.Running;
			}
		
			if(MonitoredControlledElement.Value.ControlledElementState == MonitoredControlledElement.Value.monitoredState)
			{
				//m_sentryAIController.RemoveMonitoredControlledElement(MonitoredControlledElement.Value);
			}
		
			return TaskStatus.Success;
		}
	}
}