using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units.ControllerImplementation;
using GeneralScriptableObjects;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask.CombatCheck
{
	[TaskCategory("Combat")]
	public class SentryCanUseLaserAttack : Conditional
	{
		public SharedAIController AIController;
		private SentryAIController m_sentryAIController;

		public SharedGameObject intruder;

		public override void OnAwake()
		{
			base.OnAwake();

			m_sentryAIController = (SentryAIController)AIController.Value;
		}

		public override TaskStatus OnUpdate()
		{
			float sqrDistToTar = (intruder.Value.transform.position - m_sentryAIController.transform.position).sqrMagnitude;
			if (!m_sentryAIController.LaserAttackCoolingDown && sqrDistToTar >= m_sentryAIController
			.laserAttackSettings.laserAttackRange.x * m_sentryAIController.laserAttackSettings.laserAttackRange.x 
			                                                 && sqrDistToTar <= m_sentryAIController.laserAttackSettings.laserAttackRange.y * m_sentryAIController.laserAttackSettings.laserAttackRange.y
			                                                 && !Physics2D.Linecast(m_sentryAIController.transform
			                                                 .position,intruder.Value.transform.position, 
			                                                 m_sentryAIController.laserAttackSettings.laserObstaclesLayerMask.Value))
			{
				return TaskStatus.Success;
			}
		
			return TaskStatus.Failure;
		}
	}
}