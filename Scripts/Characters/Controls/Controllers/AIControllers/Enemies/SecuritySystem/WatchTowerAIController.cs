using Characters.Enemies.Perception;
using Characters.Enemies.Weapons.Sentries;
using SceneManagement.NavigationPoints;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Controls.Controllers.AIControllers.Enemies.SecuritySystem
{
	public class WatchTowerAIController : SecuritySystemAIController
	{
		[FoldoutGroup("Settings")]
		[FoldoutGroup("Settings/Rotation Settings")]
		public ESpyBehavior m_RotationType;
		
		[FoldoutGroup("Settings/Rotation Settings")][ShowIf("ShowRotateCircleBehaviorVar")]
		[Range(30, 90)]
		public float rotationSpeed;

		[FoldoutGroup("Settings/Rotation Settings")]
		[ShowIf("ShowRotateCircleBehaviorVar")]
		public bool ClockwiseRotation;

		[FoldoutGroup("Settings/Rotation Settings")][ShowIf("ShowAnglesRotationVariables")]
		public LookAroundInfo[] angles = new LookAroundInfo[2];

		[FoldoutGroup("Settings/Rotation Settings")][ShowIf("ShowAnglesRotationVariables")]
		public EAngleRotationCompletedBehavior RotationCompleteBehavior; 

		[FoldoutGroup("Settings/Rotation Settings")][Range(0, 360)][ShowIf("ShowFixeBehaviorVar")]
		public int fixeAngle;

		[FoldoutGroup("Settings/Combat")][FoldoutGroup("Settings/Combat/LaserAttack")]
		public ELaserAttackType laserType;
	
		public bool laserAttackCoolingDown { get; private set; }
	
		[SerializeField][Range(0, 20)][FoldoutGroup("Settings/Combat/LaserAttack")]
		private float laserAttackCooldown;
	
		public float LaserAttackCooldown => laserAttackCooldown;
	
		private float m_currentCooldown = 0;
	
		[FoldoutGroup("Settings/Combat/LaserAttack")]
		[SerializeField][MinMaxSlider(0, 50, true)] private Vector2 laserAttackRange;
		public Vector2 LaserAttackRange => laserAttackRange;

		[FoldoutGroup("Settings/Combat/LaserAttack")][HideIf("IsCircularAttack")]
		public float laserStartOffset = 3;

		[FoldoutGroup("Settings/Combat/LaserAttack")]
		public float laserMovingSpeed = 1;

		[FoldoutGroup("Settings/Combat/LaserAttack")][ShowIf("IsLineAttack")]
		public float straightLaserMaxDistance = 50;

		[FoldoutGroup("Settings/Combat/LaserAttack")][ShowIf("IsCircularAttack")][Range(0, 90)][UnityEngine.Tooltip("Control how far the circular laser attack will start from the target")]
		public float circularAttackStartAngleDistance = 45;
	
		[FoldoutGroup("Settings/Combat/LaserAttack")]
		public Vector2 laserOriginOffset;
	
		private bool IsLineAttack()
		{
			return laserType == ELaserAttackType.Line;
		}
		private bool IsCircularAttack()
		{
			return laserType == ELaserAttackType.Circular;
		}

		protected override void Awake()
		{
			base.Awake();

			switch (m_RotationType)
			{
				case ESpyBehavior.Fixe:
					ChangeLookingDirection(fixeAngle);
					break;
				case ESpyBehavior.RotateBetweenAngles:
					ChangeLookingDirection(angles[0].lookingAngle);
					break;
			}
			
			InitializeBtValues();
		}
		
		
		private void Update()
		{
			if (laserAttackCoolingDown)
			{
				m_currentCooldown += Time.deltaTime;

				if (m_currentCooldown >= laserAttackCooldown) laserAttackCoolingDown = false;
			}
		}

		bool ShowRotateCircleBehaviorVar()
		{
			if (m_RotationType == ESpyBehavior.RotateCirle)
				return true;

			else return false;
		}

		bool ShowAnglesRotationVariables()
		{
			if (m_RotationType == ESpyBehavior.RotateBetweenAngles)
				return true;

			else return false;
		}

		bool ShowFixeBehaviorVar()
		{
			if (m_RotationType == ESpyBehavior.Fixe)
				return true;

			else return false;
		}

		public void LaserAttackComplete()
		{
			laserAttackCoolingDown = true;
			m_currentCooldown = 0;
		}
	}

	public enum ESpyBehavior { Fixe, RotateCirle, RotateBetweenAngles }

	public enum EAngleRotationCompletedBehavior { PingPong, Loop }
}