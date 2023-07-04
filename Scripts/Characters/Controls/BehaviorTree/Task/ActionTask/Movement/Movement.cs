using System;
using System.Collections;
using System.Linq;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;
using GeneralScriptableObjects;
using Pathfinding;
using Tools.Extension;
using UnityEngine;
using Utilities;
using Action = BehaviorDesigner.Runtime.Tasks.Action;
using Random = UnityEngine.Random;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Movement
{
	public abstract class Movement : Action
	{
		public SharedAIController AIController;

		protected UnitAIController m_unitAIController;
		
		protected GridGraph m_areaGraph;
		protected GridGraph m_mainGraph;

		protected bool pathImpossible;
		protected bool currentPathGotBlocked;
		protected Path path;
		
		public EMovementType movementType = EMovementType.Patrol;

		private int m_movementSpeed;

		protected bool alreadyAtLocation;
	
		protected bool calculatingPath;
		protected bool followingPath;
	
		protected int currentWaypoint;
		private bool m_reachedEndOfPath;

		public float stopDistance = 1f;
		public float nextWaypointDistance = .2f;

		private Vector2 m_movingDirection;

		protected Vector2 endLocationToMoveTo;

		protected bool reachedLocation;

		protected const float SmoothTime = .1f;
	
		protected float currentVelocity;

		public bool faceMovingDirection = true;
		
		public enum EMovementType
		{
			Patrol,
			Alert
		}

		public override void OnAwake()
		{
			base.OnAwake();
			m_unitAIController = (UnitAIController) AIController.Value;

			AIMovementSettingsSO AIMovementSettings;
			
			switch (movementType)
			{
				case EMovementType.Patrol:
					AIMovementSettings = m_unitAIController.patrolMovementSettings;
					break;
				case EMovementType.Alert:
					AIMovementSettings = m_unitAIController.alertMovementSettings;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			
			m_movementSpeed = AIMovementSettings.useRandomSpeed? Random.Range(AIMovementSettings.randomSpeed.x, 
				AIMovementSettings.randomSpeed.y) : AIMovementSettings.fixedSpeed;
			
			m_areaGraph = (GridGraph) AstarPath.active.data.graphs[1];
			m_mainGraph = AstarPath.active.data.gridGraph;
		}

		public override void OnStart()
		{
			followingPath = false;
			pathImpossible = false;
			currentPathGotBlocked = false;
			AstarPath.OnGraphsUpdated += OnGraphsUpdated;
		}

		public override void OnEnd()
		{
			base.OnEnd();
			AstarPath.OnGraphsUpdated -= OnGraphsUpdated;
		}

		private void OnGraphsUpdated(AstarPath script)
		{
			StartCoroutine(GraphUpdated());
		}

		private IEnumerator GraphUpdated()
		{
			yield return null;
		
			if (followingPath)
			{
				var endPos = PathfindingUtilities.GetNearestNavigableNode(path.vectorPath[path.vectorPath.Count - 1], GraphMask
					.FromGraphName("AreaGraph"));
				var currentPos = PathfindingUtilities.GetNearestNavigableNode(transform.position, GraphMask.FromGraphName("AreaGraph"));
				if(currentPos.Area != endPos.Area) currentPathGotBlocked = true;
			}
		}

		public override void OnFixedUpdate()
		{
			if (followingPath)
			{
				MoveAI();
			}
		}

		private void RecalculateCurrentPath()
		{
			m_unitAIController.Seeker.StartPath(path.vectorPath[currentWaypoint], path.vectorPath.Last(), 
				CurrentPathRecalculated);
		}

		private void CurrentPathRecalculated(Path p)
		{
			if (!p.error)
			{
			
			}
		}

		protected void MoveAI()
		{
			if (path == null || path.vectorPath.Count <2)
			{
				// We have no path to follow yet, so don't do anything
				return;
			}
		
			m_reachedEndOfPath = false;
		
			float distanceToWaypoint;
		
			// If you want maximum performance you can check the squared distance instead to get rid of a
			// square root calculation. But that is outside the scope of this tutorial.
			distanceToWaypoint = (transform.position - path.vectorPath[currentWaypoint]).sqrMagnitude;

			if ((path.vectorPath.Last() - transform.position).sqrMagnitude < stopDistance * stopDistance)
			{
				m_reachedEndOfPath = true;
				path.Release(this);
				path = null;

				AIController.Value.Rb2d.velocity = Vector2.zero;

				followingPath = false;
				reachedLocation = true;
				return;
			}

			if (distanceToWaypoint < nextWaypointDistance)
			{
				currentWaypoint++;
			}

			if (path != null) 
			{
				// Slow down smoothly upon approaching the end of the path
				// This value will smoothly go from 1 to 0 as the agent approaches the last waypoint in the path.
				var speedFactor = m_reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance * nextWaypointDistance) : 1f;

				// Direction to the next waypoint
				// Normalize it so that it has a length of 1 world unit
				m_movingDirection = (path.vectorPath[currentWaypoint] - transform.position).normalized;

				// Multiply the direction by our desired speed to get a velocity
				Vector2 velocity = m_movingDirection * m_movementSpeed * speedFactor * MovementExtension.GetVerticalSpeedModifier(Mathf.Abs(m_movingDirection.y));

				AIController.Value.Rb2d.velocity = velocity;

				if (faceMovingDirection && AIController.Value.Rb2d.velocity.magnitude > .1f)
				{
					var a = Mathf.SmoothDampAngle(AIController.Value.LookingDirection,
						MathCalculation.ConvertDirectionToAngle(AIController.Value.Rb2d.velocity.normalized),
						ref currentVelocity, SmoothTime, 360, Time.fixedDeltaTime);
					AIController.Value.ChangeLookingDirection(a);
				}
			}
		}
	}
}