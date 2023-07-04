using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.SecuritySystem;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask.CombatCheck
{
	[TaskCategory("Combat")]
	public class WatchTowerCanUseLaserAttack : Conditional
	{
		public SharedAIController AIController;
		WatchTowerAIController m_WatchTowerAIController;

		public SharedGameObject intruder;

		public LayerMask laserObstaclesLayerMask;

		public override void OnAwake()
		{
			base.OnAwake();

			m_WatchTowerAIController = (WatchTowerAIController)AIController.Value;
		}

		public override TaskStatus OnUpdate()
		{
			float sqrDistToTar = (intruder.Value.transform.position - m_WatchTowerAIController.transform.position).sqrMagnitude;
			if (!m_WatchTowerAIController.laserAttackCoolingDown && sqrDistToTar >= m_WatchTowerAIController.LaserAttackRange.x * m_WatchTowerAIController.LaserAttackRange.x 
			                                                     && sqrDistToTar <= m_WatchTowerAIController.LaserAttackRange.y * m_WatchTowerAIController.LaserAttackRange.y
			                                                     && !Physics2D.Linecast(m_WatchTowerAIController.transform.position,intruder.Value.transform.position, laserObstaclesLayerMask))
			{
				return TaskStatus.Success;
			}
		
			return TaskStatus.Failure;
		}
	}
}