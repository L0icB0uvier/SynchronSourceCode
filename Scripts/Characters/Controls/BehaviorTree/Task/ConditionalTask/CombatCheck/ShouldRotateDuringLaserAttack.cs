using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units.ControllerImplementation;
using Characters.Enemies.Weapons.Sentries;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask.CombatCheck
{
	public class ShouldRotateDuringLaserAttack : Conditional
	{
		public SharedAIController AIController;

		private SentryAIController m_sentryAIController;

		public override void OnAwake()
		{
			base.OnAwake();
			m_sentryAIController = (SentryAIController)AIController.Value;
		}

		public override TaskStatus OnUpdate()
		{
			return m_sentryAIController.laserAttackSettings.laserType == ELaserAttackType.Follow ? TaskStatus.Success : TaskStatus
			.Failure;
		}
	}
}