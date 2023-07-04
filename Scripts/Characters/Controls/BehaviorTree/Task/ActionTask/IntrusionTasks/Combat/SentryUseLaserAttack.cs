using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.BehaviorTree.Task.ConditionalTask.PerceptionCheck;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units.ControllerImplementation;
using Characters.Enemies.Weapons.Sentries;
using Lean.Pool;
using UnityEngine;
using Utilities;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.IntrusionTasks.Combat
{
	[TaskCategory("Behavior/Combat")]
	public class SentryUseLaserAttack : Action
	{
		public SharedAIController AIController;
		private SentryAIController m_sentryAIController;

		public SharedGameObject target;

		private LaserAttack m_laser;
		private SeeTarget m_see;

		private bool m_laserAttackComplete = false;
		
		public override void OnAwake()
		{
			base.OnAwake();

			m_sentryAIController = (SentryAIController)AIController.Value;
		
			m_see = Owner.FindTask<SeeTarget>();
		}

		public override void OnStart()
		{
			base.OnStart();
			m_laserAttackComplete = false;
			m_sentryAIController.StartLaserAttack();
			m_laser = LeanPool.Spawn(PrefabInstantiationUtility.GetGameObjectRefByName("LaserAttack")).GetComponent<LaserAttack>();
		
			m_laser.InitialiseLaser(m_sentryAIController.transform, target.Value.transform, m_sentryAIController.GetLaserOrigin(), 
				m_sentryAIController.laserAttackSettings, OnLaserAttackComplete);
		
			m_see.canSee = false;
		}

		public override TaskStatus OnUpdate()
		{
			if (!m_laserAttackComplete) return TaskStatus.Running;
		
			m_see.canSee = true;
		
			m_sentryAIController.LaserAttackComplete();
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
		}
		
		public override void OnBehaviorComplete()
		{
			base.OnBehaviorComplete();
			if (!m_laser) return;
			m_laser.ReturnToPool();
		}
	}
}