using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units.ControllerImplementation;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask.Behavior
{
	[TaskCategory("Job")][TaskName("Should Monitor?")] 
	public class IsMonitoredDistractingMachineOn : Conditional
	{
		public SharedAIController AIController;

		private SentryAIController m_sentryAIController;

		public SharedVector2 ControlledElementLocation;
		
		public override void OnAwake()
		{
			base.OnAwake();
			m_sentryAIController = (SentryAIController)AIController.Value;
		}

		public override TaskStatus OnUpdate()
		{
			if (m_sentryAIController.DistractingMachine == null) return TaskStatus.Failure;
			
			ControlledElementLocation.Value = m_sentryAIController.DistractingMachine.transform.position;
			return TaskStatus.Success;
		}
	}
}