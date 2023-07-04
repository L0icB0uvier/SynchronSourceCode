using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units.ControllerImplementation;
using Gameplay.InteractionSystem;
using Gameplay.InteractionSystem.Switch;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Interaction
{
	public class InteractWithSwitch : Action
	{
		public SharedAIController AIController;

		private SentryAIController m_sentryAIController;

		public override void OnAwake()
		{
			base.OnAwake();
			m_sentryAIController = (SentryAIController) AIController.Value;
		}

		public override TaskStatus OnUpdate()
		{
			m_sentryAIController.Interact();

			return TaskStatus.Success;
		}
	}
}