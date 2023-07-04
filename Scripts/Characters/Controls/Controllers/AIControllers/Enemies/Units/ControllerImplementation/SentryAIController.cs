using Characters.Enemies.Weapons.Sentries;
using Gameplay.PoweredObjects.ControlledPoweredObjects;
using GeneralScriptableObjects.EnemyDataContainers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Controls.Controllers.AIControllers.Enemies.Units.ControllerImplementation
{
	public class SentryAIController : UnitAIController
	{
		[FoldoutGroup("AI/Combat")]
		public LaserAttackSettings laserAttackSettings;
		
		public bool LaserAttackCoolingDown { get; private set; }
		
		private float m_currentCooldown = 0;

		[SerializeField][FoldoutGroup("Components")] private GameObject investigateSign;
		
		public DistractingMachine DistractingMachine { get; private set; }

		public void MonitoredMachineTurnedOn(DistractingMachine distractingMachine)
		{
			if (JobsAssigned.Count == 0) return;
			var turnOnJobs = JobsAssigned.FindAll(x => x.JobType == UnitJob.EJobType.TurnOnDistractingMachine);

			if (turnOnJobs.Count == 0) return;
			
			foreach (var job in turnOnJobs)
			{
				var turnOffJob = (TurnOnDistractingMachineJob)job;
				if (turnOffJob.DistractingMachine == distractingMachine)
				{
					JobsAssigned.Remove(turnOffJob);
					return;
				}
			}
		}

		public void MonitoredMachineTurnedOff(DistractingMachine distractingMachine)
		{
			DistractingMachine = distractingMachine;
			TurnOnDistractingMachineJob job = new TurnOnDistractingMachineJob(distractingMachine);
			JobsAssigned.Add(job);
		}
		
		private static readonly int usingLaser = Animator.StringToHash("UsingLaser");
		
		protected override void Start()
		{
			base.Start();
			LaserAttackCoolingDown = false;
		}

		public void StartLaserAttack()
		{
			m_Animator.SetBool(usingLaser, true);
		}

		public void LaserAttackComplete()
		{
			m_Animator.SetBool(usingLaser, false);
			LaserAttackCoolingDown = true;
			m_currentCooldown = 0;
		}

		private void Update()
		{
			if (LaserAttackCoolingDown)
			{
				m_currentCooldown += Time.deltaTime;

				if (m_currentCooldown >= laserAttackSettings.laserAttackCooldown) LaserAttackCoolingDown = false;
			}
		}

		protected override void InitializeBtValues()
		{
			base.InitializeBtValues();
			enemyBehaviorTree.SetVariableValue("InvestigateSign", investigateSign);
		}

		public Vector2 GetLaserOrigin()
		{
			if (LookingDirection <= 22.5f || LookingDirection > 337.5f)
			{
				return (Vector2)transform.position + laserAttackSettings.originLocation[0];
			}
			if(LookingDirection <= 67.5f && LookingDirection > 22.5f)
			{
				return (Vector2)transform.position + laserAttackSettings.originLocation[1];
			} 
			if(LookingDirection <= 112.5f && LookingDirection > 67.5f)
			{
				return (Vector2)transform.position + laserAttackSettings.originLocation[2]; 
			}
			if(LookingDirection <= 157.5f && LookingDirection > 112.5f)
			{
				return (Vector2)transform.position + laserAttackSettings.originLocation[3];
			}
			if(LookingDirection <= 202.5f && LookingDirection > 157.5f)
			{
				return (Vector2)transform.position + laserAttackSettings.originLocation[4];
			}
			if(LookingDirection <= 247.5f && LookingDirection > 202.5f)
			{
				return (Vector2)transform.position + laserAttackSettings.originLocation[5];
			}
			if(LookingDirection <= 292.5f && LookingDirection > 247.5f)
			{
				return (Vector2)transform.position + laserAttackSettings.originLocation[6];
			}
		
			return (Vector2)transform.position + laserAttackSettings.originLocation[7];
		}
	}
}