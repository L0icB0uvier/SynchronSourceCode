using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.PlayerControllers.Hicks;
using GeneralScriptableObjects;
using Pathfinding;
using Tools.Extension;
using UnityEngine;
using Utilities;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Movement
{
	[TaskCategory("Movement/Move")]
	[TaskDescription("Player character follow transform")]
	public class PlayerCharacterFollowTransform : Action
	{
		public SharedAIController AIController;

		private PlayerAIController m_playerAIController;

		public SharedTransform transformToFollow;

		private Path m_path;

		public FloatVariable movementSpeed;
		
		private bool m_followingPath;

		private int m_currentWaypoint;
		private bool m_reachedEndOfPath;

		public float stopDistance = 1f;
		public float nextWaypointDistance = .2f;

		private Vector2 m_movingDirection;

		private Vector2 m_currentLocationToMoveTo;

		public float requestPathThreshold = 1;

		public bool rotateTowardPathBeforeMoving;
		public float smoothTime;

		private float m_angleToStartPath;
		private float m_currentVelocity;

		public FloatVariable lookingDirection;
		public BoolVariable isMoving;

		public override void OnAwake()
		{
			base.OnAwake();

			m_playerAIController = (PlayerAIController)AIController.Value;
		}

		public override void OnStart()
		{
			m_followingPath = false;
			RequestPath();
		}

		public override TaskStatus OnUpdate()
		{
			if (ShouldRecalculatePath())
			{
				RequestPath();
			}
			
			return TaskStatus.Running;
		}

		public override void OnFixedUpdate()
		{
			if (m_followingPath)
			{
				if (m_currentWaypoint == 1 && rotateTowardPathBeforeMoving && !FacingStartPath())
				{
					AIController.Value.ChangeLookingDirection(Mathf.SmoothDampAngle(AIController.Value.LookingDirection,
						m_angleToStartPath, ref m_currentVelocity, smoothTime, 90, Time.fixedDeltaTime));
				}

				else
				{
					MoveAI();
				}
			}
		}

		bool FacingStartPath()
		{
			Vector2 directionToPathStart = (m_path.vectorPath[1] - AIController.Value.transform.position).normalized;
			m_angleToStartPath = MathCalculation.ConvertDirectionToAngle(directionToPathStart);
			float angleDif = Mathf.Abs(Mathf.DeltaAngle(AIController.Value.LookingDirection, m_angleToStartPath));
			return angleDif < 1;
		}

		bool ShouldRecalculatePath()
		{
			Vector2 transformPosition
				= transformToFollow.Value.position;
			return transformPosition != m_currentLocationToMoveTo && !((transformPosition - m_currentLocationToMoveTo).sqrMagnitude < requestPathThreshold * requestPathThreshold);
		}

		private void RequestPath()
		{
			m_currentLocationToMoveTo = transformToFollow.Value.position;
			m_playerAIController.Seeker.StartPath(transform.position, m_currentLocationToMoveTo, PathFound);
		}

		//A path was found, create a new path and set followingPath to true
		public void PathFound(Path p)
		{
			//Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

			// Path pooling. To avoid unnecessary allocations paths are reference counted.
			// Calling Claim will increase the reference count by 1 and Release will reduce
			// it by one, when it reaches zero the path will be pooled and then it may be used
			// by other scripts. The ABPath.Construct and Seeker.StartPath methods will
			// take a path from the pool if possible. See also the documentation page about path pooling.
			p.Claim(this);

			if (!p.error)
			{
				if (m_path != null) m_path.Release(this);
				m_playerAIController.Seeker.drawGizmos = true;
				m_path = p;
				// Reset the waypoint counter so that we start to move towards the first point in the path
				m_currentWaypoint = 1;

				isMoving.SetValue(true);

				m_followingPath = true;
			}

			else
			{
				p.Release(this);
			}
		}

		private void MoveAI()
		{
			if (m_path == null || m_path.vectorPath.Count < 2)
			{
				// We have no path to follow yet, so don't do anything
				return;
			}

			// Check in a loop if we are close enough to the current waypoint to switch to the next one.
			// We do this in a loop because many waypoints might be close to each other and we may reach
			// several of them in the same frame.
			m_reachedEndOfPath = false;
			// The distance to the next waypoint in the path
			float distanceToWaypoint;

			while (true)
			{
				// If you want maximum performance you can check the squared distance instead to get rid of a
				// square root calculation. But that is outside the scope of this tutorial.
				distanceToWaypoint = (m_path.vectorPath[m_currentWaypoint] - transform.position).sqrMagnitude;

				if (m_currentWaypoint + 1 == m_path.vectorPath.Count)
				{
					if (distanceToWaypoint < stopDistance * stopDistance)
					{
						// Set a status variable to indicate that the agent has reached the end of the path.
						// You can use this to trigger some special code if your game requires that.
						m_reachedEndOfPath = true;
						m_path.Release(this);
						m_path = null;

						m_followingPath = false;
						isMoving.SetValue(false);
						break;
					}
					
					break;
				}

				else
				{
					if (distanceToWaypoint < nextWaypointDistance)
						m_currentWaypoint++;

					else
						break;
				}

			}

			if (m_path != null)
			{
				// Slow down smoothly upon approaching the end of the path
				// This value will smoothly go from 1 to 0 as the agent approaches the last waypoint in the path.
				var speedFactor = m_reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;

				// Direction to the next waypoint
				// Normalize it so that it has a length of 1 world unit
				m_movingDirection = (m_path.vectorPath[m_currentWaypoint] - transform.position).normalized;

				float angle = Mathf.Atan2(m_movingDirection.y, m_movingDirection.x) * Mathf.Rad2Deg - 90f;

				if (angle < 0)
					angle += 360;

				//Check if the unit is facing the right direction, if it does move the unit
				//if (MathCalculation.ApproximatelyEqualFloat(lookingDirectionAngle, angle, 45))
				//{

				// Multiply the direction by our desired speed to get a velocity
				AIController.Value.Rb2d.MovePosition((Vector2)AIController.Value.transform.position +
				                                     m_movingDirection * movementSpeed.Value * speedFactor *
				                                     MovementExtension.GetVerticalSpeedModifier(Mathf.Abs
					                                     (m_movingDirection.y)) * Time.deltaTime);
				
				lookingDirection.SetValue(MathCalculation.ConvertDirectionToAngle(m_movingDirection));
			}
		}
	}
}