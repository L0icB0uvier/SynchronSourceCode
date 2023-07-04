using System;
using System.Collections.Generic;
using Characters.Controls.Controllers.AIControllers.Enemies.Units.ControllerImplementation;
using Characters.Enemies.Perception;
using Gameplay.InteractionSystem.Interacters;
using Gameplay.InteractionSystem.Switch;
using GeneralScriptableObjects;
using Pathfinding;
using SceneManagement.NavigationPoints;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Characters.Controls.Controllers.AIControllers.Enemies.Units
{
	public abstract class UnitAIController : EnemyAIController
	{
		[SerializeField][FoldoutGroup("AI")]
		private EUnitType unitType;

		public EUnitType UnitType => unitType;

		private Seeker m_seeker;
		public Seeker Seeker => m_seeker;

		[FoldoutGroup("AI/MovementSettings")]
		public AIMovementSettingsSO patrolMovementSettings;
		
		[FoldoutGroup("AI/MovementSettings")]
		public AIMovementSettingsSO alertMovementSettings;

		[FoldoutGroup("AI/MovementSettings")]
		public float searchMaxDistanceFromOrigin = 30;

		[ShowInInspector][ReadOnly][FoldoutGroup("Debug")]
		private EUnitBehavior m_unitBehavior;
		public EUnitBehavior UnitCurrentBehavior => m_unitBehavior;

		[SerializeField][FoldoutGroup("AI")][ShowIf("ShowPatrolPoints")]
		public PatrolPath[] patrol = new PatrolPath[1];

		public PatrolInfo[] CurrentPathPatrolPoints => patrol[m_currentPathIndex].patrolPoints;

		[ShowInInspector][ReadOnly][FoldoutGroup("Debug")]
		private int m_currentPathIndex;
	
		[ShowInInspector][ReadOnly][FoldoutGroup("Debug")]
		private int m_currentPatrolPointIndex;

		public int CurrentPatrolPointIndex
		{
			get => m_currentPatrolPointIndex;
			set => m_currentPatrolPointIndex = value;
		}

		public int PingPongDirection { get; private set; } = 1;
		
		public ConnectionInterface ConnectionInterface { get; private set; }

		[SerializeField][FoldoutGroup("Components")] private Interacter _unitInteracter;
		public Interacter UnitInteracter => _unitInteracter;
		
		[ShowInInspector][FoldoutGroup("Debug")]
		private List<UnitJob> m_jobsAssigned = new List<UnitJob>();
		public List<UnitJob> JobsAssigned => m_jobsAssigned;
	
		private GridGraph m_mainGraph;
		private GridGraph m_areaGraph;

		private uint m_mainGraphCurrentArea;
		private uint m_areaGraphCurrentArea;
	
		public uint MainGraphCurrentArea => m_mainGraphCurrentArea;
		public uint AreaGraphCurrentArea => m_areaGraphCurrentArea;

		public UnityEvent OnJobCompleted;
		
		private static readonly int movementSpeed = Animator.StringToHash("MovementSpeed");
		private static readonly int direction = Animator.StringToHash("Direction");

		protected override void Awake()
		{
			base.Awake();
			m_seeker = GetComponent<Seeker>();
			ConnectionInterface = GetComponentInChildren<ConnectionInterface>();
		}

		protected override void Start()
		{
			base.Start();
		
			m_mainGraph = AstarPath.active.data.gridGraph;
			m_areaGraph = (GridGraph) AstarPath.active.data.graphs[1];
		}

		protected void LateUpdate()
		{
			m_Animator.SetFloat(direction, LookingDirection);

			if (Rb2d.velocity.sqrMagnitude > 0)
			{
				Animator.SetFloat(movementSpeed, Rb2d.velocity.magnitude);
			}

			else
			{
				Animator.SetFloat(movementSpeed, 0);
			}
		}

		public void GetGraphAreas()
		{
			m_mainGraph ??= AstarPath.active.data.gridGraph;
			m_areaGraph ??= (GridGraph) AstarPath.active.data.graphs[1];
		
			Vector3 unitPos = transform.position;
			
			var nnMain = new NNConstraint
			{
				constrainWalkability = true,
				walkable = true,
				graphMask = GraphMask.FromGraphName("MainGraph"),
			};
			
			var nnArea = new NNConstraint
			{
				constrainWalkability = true,
				walkable = true,
				graphMask = GraphMask.FromGraphName("AreaGraph"),
			};
			
			GraphNode mainGraphNode = m_mainGraph.GetNearest(unitPos, nnMain).node;
			GraphNode areaGraphNode = m_areaGraph.GetNearest(unitPos, nnArea).node;

			m_mainGraphCurrentArea = mainGraphNode.Area;
			m_areaGraphCurrentArea = areaGraphNode.Area;
		}
		
		public void Interact()
		{
			_unitInteracter.TryInteraction();
		}

		public void AssignJob(UnitJob job)
		{
			m_jobsAssigned.Add(job);
		}

		public void ResetJobs()
		{
			m_jobsAssigned.Clear();
		}

		public void JobCompleted(UnitJob job)
		{
			CurrentArea.UnitReportJobCompleted(this);
			if (m_jobsAssigned.Count == 0) return;
			m_jobsAssigned.Remove(job);
		}

		public void ChangeCurrentBehavior(EUnitBehavior newBehavior)
		{
			m_unitBehavior = newBehavior;
		}

		public void SetCurrentPatrolPointUnreachable()
		{
			patrol[m_currentPathIndex].patrolPoints[m_currentPatrolPointIndex].accessible = false;
		}

		public void IncrementPatrolPathIndex()
		{
			if (m_currentPathIndex >= patrol.Length - 1) return;

			m_currentPathIndex++;
		}

		public EPatrolBehavior GetCurrentPathPatrolBehavior()
		{
			return patrol[m_currentPathIndex].patrolBehavior;
		}

		public PatrolInfo GetCurrentPathPatrolPointAtIndex(int i)
		{
			return i > patrol[m_currentPathIndex].patrolPoints.Length - 1 ? null : patrol[m_currentPathIndex].patrolPoints[i];
		}
		public PatrolInfo GetPathCurrentPatrolPoint()
		{
			return patrol[m_currentPathIndex].patrolPoints[m_currentPatrolPointIndex];
		}

		public void InversePingPongDirection()
		{
			PingPongDirection = PingPongDirection == 1 ? -1 : 1;
		}
		
#if UNITY_EDITOR
		private bool ShowPatrolPoints()
		{
			return defaultBehavior == EEnemyDefaultBehavior.Default? true : false;
		}

		private void OnDrawGizmosSelected()
		{
			if (patrol[m_currentPathIndex].patrolPoints.Length == 0)
				return;

			foreach(var patrolPoint in patrol[m_currentPathIndex].patrolPoints)
			{
				if (!patrolPoint.patrolPoint)
					return;
			}

			switch (patrol[m_currentPathIndex].patrolBehavior)
			{
				case EPatrolBehavior.Fixe:
					return;

				case EPatrolBehavior.Loop:
				
					for (int i = 0; i < patrol[m_currentPathIndex].patrolPoints.Length; i++)
					{
						if (i == patrol[m_currentPathIndex].patrolPoints.Length - 1)
						{
							Gizmos.DrawLine(patrol[m_currentPathIndex].patrolPoints[i].patrolPoint.transform.position, patrol[m_currentPathIndex].patrolPoints[0].patrolPoint.transform.position);
						}

						else
						{
							Gizmos.DrawLine(patrol[m_currentPathIndex].patrolPoints[i].patrolPoint.transform.position, patrol[m_currentPathIndex].patrolPoints[i + 1].patrolPoint.transform.position);
						}

					}
				
					break;

				case EPatrolBehavior.PingPong:

					for (int i = 0; i < patrol[m_currentPathIndex].patrolPoints.Length - 1; i++)
					{
						Gizmos.DrawLine(patrol[m_currentPathIndex].patrolPoints[i].patrolPoint.transform.position, patrol[m_currentPathIndex].patrolPoints[i + 1].patrolPoint.transform.position);
					}

					break;
			}
		}
#endif
	}

	[Serializable]
	public class PatrolInfo
	{
		public PatrolPoint patrolPoint;
		public bool accessible = true;
	}

	[Serializable]
	public class PatrolPath
	{
		public EPatrolBehavior patrolBehavior;
		public PatrolInfo[] patrolPoints = new PatrolInfo[1];
	}

	public enum EPatrolBehavior {Fixe, PingPong, Loop, Random}
	public enum EUnitType {Sentry, MaintenanceUnit}
}