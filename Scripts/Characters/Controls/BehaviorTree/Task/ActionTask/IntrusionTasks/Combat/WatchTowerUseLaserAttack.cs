using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.BehaviorTree.Task.ConditionalTask.PerceptionCheck;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.SecuritySystem;
using Characters.Enemies.Weapons.Sentries;
using Lean.Pool;
using UnityEngine;
using Utilities;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.IntrusionTasks.Combat
{
	[TaskCategory("Behavior/Combat")]
	public class WatchTowerUseLaserAttack : Action
	{
		public SharedAIController AIController;
		private WatchTowerAIController m_watchTowerAIController;

		public SharedGameObject target;

		private LaserAttack m_laser;
		private SeeTarget m_see;

		private bool m_laserAttackComplete = false;
		
		public override void OnAwake()
		{
			base.OnAwake();

			m_watchTowerAIController = (WatchTowerAIController)AIController.Value;
		
			m_see = Owner.FindTask<SeeTarget>();
		}

		public override void OnStart()
		{
			base.OnStart();
			m_laserAttackComplete = false;
			m_laser = LeanPool.Spawn(PrefabInstantiationUtility.GetGameObjectRefByName("LaserAttack")).GetComponent<LaserAttack>();

			var watchTowerTransform = m_watchTowerAIController.transform;
			/*m_laser.InitialiseLaser(watchTowerTransform, target.Value.transform, (Vector2)watchTowerTransform.position + m_watchTowerAIController.laserOriginOffset, 
				m_watchTowerAIController.laserStartOffset, m_watchTowerAIController.laserMovingSpeed, OnLaserAttackComplete);*/
		
			m_see.canSee = false;
		}

		public override TaskStatus OnUpdate()
		{
			if (!m_laserAttackComplete) return TaskStatus.Running;
			
			m_watchTowerAIController.LaserAttackComplete();
		
			return TaskStatus.Success;
		}

		private void OnLaserAttackComplete()
		{
			m_laserAttackComplete = true;
		}

		public override void OnConditionalAbort()
		{
			base.OnConditionalAbort();
			m_laserAttackComplete = true;
			m_see.canSee = true;
			m_laser.StopLaser();
			m_laser = null;
		}

		public override void OnBehaviorComplete()
		{
			base.OnBehaviorComplete();
			if (!m_laser) return;
			m_laser.ReturnToPool();
		}
	}
}