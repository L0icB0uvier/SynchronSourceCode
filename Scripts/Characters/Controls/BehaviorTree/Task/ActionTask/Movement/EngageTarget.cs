using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;
using Gameplay.EnergySystem.EnergyProduction;
using Gameplay.InteractionSystem;
using Gameplay.InteractionSystem.Interactables;
using Gameplay.InteractionSystem.Switch;
using Gameplay.PoweredObjects.ControlledPoweredObjects;
using GeneralScriptableObjects;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Movement
{
	[TaskCategory("Movement/Move")]
	[TaskDescription("Move to an object")]
	public class EngageTarget : Movement
	{
		public SharedGameObject target;

		private bool m_isAtTargetRange;

		private GraphMask m_pathGraphMask;
	
		public LayerMaskVariable controlledElementLayerMask;

		private Vector2 m_currentPathTargetLocation;

		private Collider2D[] m_controlledElementCollider = new Collider2D[1];

		private PathBlockingMovingPoweredSystem m_movingPoweredSystemOnPath;

		private MovingPoweredSystemInteractable m_interactable;

		private bool m_waitForJobComplete;
		private EJobInWait m_jobInWait;
		private bool m_waitForControlledElementTransition;

		public float recalculatePathThreshold = 2;

		public UnityEvent onBlockingElementOpened = new UnityEvent();
		public UnityEvent OnPowerJobCompleted = new UnityEvent();

		private EMoveToLocations m_moveToCurrentLocation;

		enum EJobInWait
		{
			PowerSocket = 1,
			InteractWithSwitch = 2,
			PowerSocketAndInteractWithSwitch = 3
		}

		private enum EMoveToLocations
		{
			EndLocation = 1 << 0,
			Switch = 1 << 1,
			BlockingControlledElement = 1 << 2,
		}

		public override void OnStart()
		{
			base.OnStart();

			m_isAtTargetRange = (target.Value.transform.position - transform.position).sqrMagnitude < stopDistance * stopDistance;

			m_moveToCurrentLocation = EMoveToLocations.EndLocation;
			onBlockingElementOpened.RemoveAllListeners();
			OnPowerJobCompleted.RemoveAllListeners();
			reachedLocation = false;
			m_interactable = null;
			m_movingPoweredSystemOnPath = null;
		
			if (path != null)
			{
				path.Release(this);
				path = null;
			}

			if(!m_isAtTargetRange) RequestPathToEndLocation();
		}

		public override TaskStatus OnUpdate()
		{
			if (pathImpossible) return TaskStatus.Failure;
			if (calculatingPath) return TaskStatus.Running;

			if (currentPathGotBlocked)
			{
				RequestPathToEndLocation();
				currentPathGotBlocked = false;
				return TaskStatus.Running;
			}

			switch (m_moveToCurrentLocation)
			{
				case EMoveToLocations.EndLocation:
					if ((m_currentPathTargetLocation - (Vector2)target.Value.transform.position).sqrMagnitude >
					    recalculatePathThreshold * recalculatePathThreshold)
					{
						m_currentPathTargetLocation = target.Value.transform.position;
						RequestPathToEndLocation();
					}
				
					if (reachedLocation)
					{
						m_isAtTargetRange = true;
						OnPowerJobCompleted.Invoke();
						return TaskStatus.Running;
					}

					return TaskStatus.Running;

				case EMoveToLocations.Switch:
					if (!reachedLocation) return TaskStatus.Running;
					if (!m_waitForControlledElementTransition) return ManageSwitchInteraction();
					if (m_movingPoweredSystemOnPath.ControlledElementState != EControlledElementState.AlteredState) return TaskStatus.Running;
					m_waitForControlledElementTransition = false;
					RequestPathToEndLocation();
					return TaskStatus.Running;

				case EMoveToLocations.BlockingControlledElement:
				{
					if (!reachedLocation) return TaskStatus.Running;
					if (m_waitForJobComplete)
					{
						switch (m_jobInWait)
						{
							case EJobInWait.PowerSocket:
								if (!m_movingPoweredSystemOnPath.Powered) return TaskStatus.Running;

								/*switch (m_unitAIController.ConnectionInterface.interfaceState)
								{
									case EInterfaceState.Default:
										RequestPathToSwitch(m_interactable);
										m_waitForJobComplete = false;
										return TaskStatus.Running;

									case EInterfaceState.Connecting:
									{
										return TaskStatus.Running;
									}
									case EInterfaceState.Connected:
										if (!m_waitForControlledElementTransition) return ManageSwitchInteraction();
										if (m_movingPoweredSystemOnPath.ControlledElementState != EControlledElementState.AlteredState) return 
											TaskStatus.Running;
										m_waitForControlledElementTransition = false;
										RequestPathToEndLocation();
										m_waitForJobComplete = false;
										return TaskStatus.Running;
								}*/
							
								return TaskStatus.Running;
						
							case EJobInWait.InteractWithSwitch:
								if (m_movingPoweredSystemOnPath.ControlledElementState != EControlledElementState.AlteredState)
									return TaskStatus.Running;
					
								onBlockingElementOpened.Invoke();
								m_waitForJobComplete = false;
								RequestPathToEndLocation();
								return TaskStatus.Running;
						
							case EJobInWait.PowerSocketAndInteractWithSwitch:
								if (!m_movingPoweredSystemOnPath.Powered || m_movingPoweredSystemOnPath.ControlledElementState !=
									EControlledElementState.AlteredState) return TaskStatus.Running;
								onBlockingElementOpened.Invoke();
								m_waitForJobComplete = false;
								RequestPathToEndLocation();
								return TaskStatus.Running;
						}
					}

					if (m_movingPoweredSystemOnPath.Powered && IsControllingSwitchAccessibleByUnit(m_movingPoweredSystemOnPath))
					{
						RequestPathToSwitch(m_interactable);
						return TaskStatus.Running;
					}
				
					if (!m_movingPoweredSystemOnPath.Powered && IsControllingSwitchAccessibleByUnit(m_movingPoweredSystemOnPath))
					{
						var unit = m_unitAIController.CurrentArea.RequestUnitForJob(m_unitAIController, EUnitType.MaintenanceUnit, m_movingPoweredSystemOnPath
							.EnergySource.transform.position);

						if (!unit) return TaskStatus.Failure;
						unit.AssignJob(new PowerSocketJob((EnergySocket)m_movingPoweredSystemOnPath.EnergySource, 
							OnPowerJobCompleted));
						m_jobInWait = EJobInWait.PowerSocket;
						m_waitForJobComplete = true;
						return TaskStatus.Running;
					}

					if (m_movingPoweredSystemOnPath.Powered && !IsControllingSwitchAccessibleByUnit(m_movingPoweredSystemOnPath))
					{
						UnitAIController unit = null;
						foreach (var s in m_movingPoweredSystemOnPath.ControlledSystemInteractables)
						{
							unit = m_unitAIController.CurrentArea.RequestUnitForJob(m_unitAIController, EUnitType.Sentry, s.transform.position);
							if (unit) break;
						}

						if (!unit) return TaskStatus.Failure;
					
						unit.AssignJob(new MonitorControlledElementJob(m_movingPoweredSystemOnPath, 
							EControlledElementState.AlteredState, onBlockingElementOpened));
						m_jobInWait = EJobInWait.InteractWithSwitch;
						m_waitForJobComplete = true;
						return TaskStatus.Running;
					}

					if (!m_movingPoweredSystemOnPath.Powered &&
					    !IsControllingSwitchAccessibleByUnit(m_movingPoweredSystemOnPath))
					{
						var poweringUnit = m_unitAIController.CurrentArea.RequestUnitForJob(m_unitAIController, EUnitType.MaintenanceUnit, m_movingPoweredSystemOnPath.EnergySource.transform.position);
						UnitAIController interactingUnit = null;
						foreach (var s in m_movingPoweredSystemOnPath.ControlledSystemInteractables)
						{
							interactingUnit = m_unitAIController.CurrentArea.RequestUnitForJob(m_unitAIController, EUnitType.Sentry, s.transform.position);
							if (interactingUnit) break;
						}

						if (!poweringUnit || !interactingUnit) return TaskStatus.Failure;
						poweringUnit.AssignJob(new PowerSocketJob((EnergySocket)m_movingPoweredSystemOnPath.EnergySource, 
							OnPowerJobCompleted));
						interactingUnit.AssignJob(new MonitorControlledElementJob(m_movingPoweredSystemOnPath, EControlledElementState.AlteredState, onBlockingElementOpened));
						m_jobInWait = EJobInWait.PowerSocketAndInteractWithSwitch;
						m_waitForJobComplete = true;
						return TaskStatus.Running;
					}
				
					return TaskStatus.Running;
				}
			
				default:
					return TaskStatus.Running;
			}
		}

		public override void OnFixedUpdate()
		{
			base.OnFixedUpdate();
			if (!m_isAtTargetRange) return;
		
			var dirToTarget = (target.Value.transform.position - AIController.Value.transform.position).normalized;
			var angleToTarget = Mathf.RoundToInt(MathCalculation.ConvertDirectionToAngle(dirToTarget));
			
			var a = Mathf.SmoothDampAngle(AIController.Value.LookingDirection,
				angleToTarget, ref currentVelocity, SmoothTime, 360, Time.fixedDeltaTime);
			AIController.Value.ChangeLookingDirection(a);
		}

		private TaskStatus ManageSwitchInteraction()
		{
			if (/*m_unitAIController.ConnectionInterface.interfaceState == EInterfaceState.Connected &&*/ 
			    m_movingPoweredSystemOnPath.ControlledElementState == EControlledElementState.DefaultState)
			{
				m_unitAIController.Interact();
				m_waitForControlledElementTransition = true;
			}

			return TaskStatus.Running;
		}

		private void RequestPathToEndLocation()
		{
			m_isAtTargetRange = false;
			endLocationToMoveTo = (Vector3)PathfindingUtilities.GetNearestNavigableNode(target.Value.transform.position, 
				GraphMask.FromGraphName("MainGraph")).position;
			m_pathGraphMask = GraphMask.FromGraphName("MainGraph");
			m_unitAIController.Seeker.graphMask = m_pathGraphMask;
			m_unitAIController.Seeker.StartPath(transform.position, endLocationToMoveTo, PathToEndLocationFound);
			calculatingPath = true;
		}
	
		private void RequestPathToSwitch(MovingPoweredSystemInteractable i)
		{
			Vector2 switchLocation =  (Vector3)PathfindingUtilities.GetNearestNavigableNode(i.transform.position, GraphMask
				.FromGraphName("MainGraph")).position;
			m_pathGraphMask = GraphMask.FromGraphName("MainGraph");
			m_unitAIController.Seeker.graphMask = m_pathGraphMask;
			m_unitAIController.Seeker.StartPath(transform.position, switchLocation, PathToSwitchFound);
			calculatingPath = true;
		}

		private void RequestPathToBlockingElement(PathBlockingMovingPoweredSystem c)
		{
			var unitNode = m_areaGraph.GetNearest(m_unitAIController.transform.position, NNConstraint.Default).node;
		
			foreach (var entrance in c.elementEntrances)
			{
				var entranceNode = m_areaGraph.GetNearest(entrance.position, NNConstraint.Default).node;
				if (unitNode.Area != entranceNode.Area) continue;
				m_unitAIController.Seeker.StartPath(transform.position, entrance.position, PathToControlledElementEntranceFound);
				calculatingPath = true;
				return;
			}
		}

		//A path was found, create a new path and set followingPath to true
		private void PathToEndLocationFound(Path p)
		{
			calculatingPath = false;
		
			if (!p.error)
			{
				if (path != null)
				{
					path.Release(this);
					path = null;
				}
				m_unitAIController.Seeker.drawGizmos = true;

				if (!IsControlledElementBlockingPath(p))
				{
					path = p;
					p.Claim(this);

					m_moveToCurrentLocation = EMoveToLocations.EndLocation;
					currentWaypoint = 1;
					reachedLocation = false;
					followingPath = true;
					return;
				}

				RequestPathToBlockingElement(m_movingPoweredSystemOnPath);
				return;
			}
		
			pathImpossible = true;
		}
	
		private void PathToSwitchFound(Path p)
		{
			calculatingPath = false;
		
			if (!p.error)
			{
				if (path != null) path.Release(this);
				m_unitAIController.Seeker.drawGizmos = true;

				path = p;
				p.Claim(this);
				currentWaypoint = 1;
			
				m_moveToCurrentLocation = EMoveToLocations.Switch;
				reachedLocation = false;
				m_waitForControlledElementTransition = false;
				followingPath = true;
				return;
			}
		
			pathImpossible = true;
		}
	
		private void PathToControlledElementEntranceFound(Path p)
		{
			calculatingPath = false;
		
			if (!p.error)
			{
				if (path != null) path.Release(this);
				m_unitAIController.Seeker.drawGizmos = true;
			
				path = p;
				p.Claim(this);
				currentWaypoint = 1;
			
				m_moveToCurrentLocation = EMoveToLocations.BlockingControlledElement;
				reachedLocation = false;
				m_waitForJobComplete = false;
				followingPath = true;
				return;
			}

			pathImpossible = true;
		}
	
		private bool IsControlledElementBlockingPath(Path p)
		{
			foreach (var node in p.path)
			{
				if (Physics2D.OverlapPointNonAlloc((Vector3) node.position, m_controlledElementCollider, controlledElementLayerMask.Value) == 0) continue;
			
				m_movingPoweredSystemOnPath = m_controlledElementCollider[0].GetComponent<PathBlockingMovingPoweredSystem>();

				if (!m_movingPoweredSystemOnPath) return false;
			
				if (m_movingPoweredSystemOnPath.ControlledElementState == EControlledElementState.DefaultState ||
				    m_movingPoweredSystemOnPath.IsTransitioning)
				{
					return true;
				}
			}
			return false;
		}
	
		private bool IsControllingSwitchAccessibleByUnit(PathBlockingMovingPoweredSystem movingPoweredSystem)
		{
			foreach (var s in movingPoweredSystem.ControlledSystemInteractables)
			{
				var n1 = m_areaGraph.GetNearest(AIController.Value.transform.position, NNConstraint.Default).node;
				var n2 = m_areaGraph.GetNearest(s.transform.position, NNConstraint.Default).node;

				if (n1.Area == n2.Area)
				{
					m_interactable = s;
					return true;
				}
			}

			return false;
		}
	}
}