using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;
using GeneralScriptableObjects;
using Pathfinding;
using UnityEngine;
using Utilities;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.IntrusionTasks.Search
{
	[TaskCategory("Behavior/Search")]
	public class FindNextSearchPoint : Action
	{
		public SharedAIController AIController;

		UnitAIController m_unitAIController;

		public SharedVector2 searchPointLocation;

		public SharedVector2 searchOrigin;
	
		public float minDistanceFromPreviousSearchPoint = 5;

		public float turnStep = 15;
	
		[UnityEngine.Tooltip("The maximum number of retries per tick (set higher if using a slow tick time)")]
		public int targetRetries;
	
		[UnityEngine.Tooltip("The amount that the agent rotates direction")]
		public float wanderRate = 1;
	
		public float minWanderDistance = 20;

		public float maxWanderDistance = 20;

		public LayerMaskVariable obstacleLayerMask;

		private RaycastHit2D[] obstacleHits = new RaycastHit2D[1];

		public DefineSearchPerimeter searchPerimeter;

		public override void OnAwake()
		{
			base.OnAwake();
			m_unitAIController = (UnitAIController)AIController.Value;
		}

		public override TaskStatus OnUpdate()
		{
			return TrySetTarget()? TaskStatus.Success : TaskStatus.Failure;
		}
	
		private bool TrySetTarget()
		{
			m_unitAIController.GetGraphAreas();
		
			var turnRight = Random.value > 0.5f;
			var direction = m_unitAIController.LookingDirection;
			var validDestination = false;
			var attempts = targetRetries;
			Vector2 searchLocation;

			while (!validDestination && attempts > 0)
			{
				var searchdir = (MathCalculation.ConvertAngleToDirection(direction) + Random.insideUnitCircle * wanderRate)
					.normalized;
				var destination = (Vector2)transform.position + searchdir * Random.Range(minWanderDistance,
					maxWanderDistance);

				searchLocation = PathfindingUtilities.GetNearestNavigableNode(destination, GraphMask.FromGraphName("AreaGraph"),
					m_unitAIController.AreaGraphCurrentArea);

				if (IsSearchLocationValid(searchLocation))
				{
					searchPointLocation.Value = searchLocation;
					searchPerimeter.AddSearchLocation(searchLocation);
					return true;
				}
			
				if (turnRight)
				{
					direction = Mathf.LerpAngle(direction, direction - turnStep, 1);
				}

				else
				{
					direction = Mathf.LerpAngle(direction, direction + turnStep, 1);
				}

				attempts--;
			}
		
			return false;
		}

		private bool IsSearchLocationValid(Vector2 location)
		{
			var sqrDistance = (location - searchOrigin.Value).sqrMagnitude;
			bool inRange = sqrDistance >= minWanderDistance * minWanderDistance && sqrDistance <= 
				m_unitAIController.searchMaxDistanceFromOrigin * m_unitAIController.searchMaxDistanceFromOrigin;
			bool clearSight = Physics2D.LinecastNonAlloc(transform.position, location, obstacleHits, 
			obstacleLayerMask.Value.value) == 0;
			bool closeToPreviousSearchLocation = searchPerimeter.SearchLocations.Any(x => (location - x).sqrMagnitude <
				minDistanceFromPreviousSearchPoint * minDistanceFromPreviousSearchPoint);
			return inRange && clearSight && !closeToPreviousSearchLocation;
		}
	}
}