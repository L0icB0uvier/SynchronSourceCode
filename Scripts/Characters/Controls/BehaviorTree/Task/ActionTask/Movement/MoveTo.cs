using System;
using System.Linq;
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

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Movement
{
	[TaskCategory("Movement/Move")]
	[TaskDescription("Move to a location")]
	public class MoveTo : Movement
	{
		public SharedVector2 location;

		private GraphMask m_pathGraphMask;
	
		public LayerMaskVariable controlledElementLayerMask;

		private Collider2D[] m_controlledElementCollider = new Collider2D[1];

		private PathBlockingMovingPoweredSystem m_movingPoweredSystemOnPath;

		private MovingPoweredSystemInteractable m_interactable;

		private EControlledElementState m_controlledElementDesiredState;
		
		private EJobInWait m_jobsPending;

		public UnityEvent onBlockingElementOpened = new UnityEvent();
		public UnityEvent OnPowerJobCompleted = new UnityEvent();

		private EMoveToState m_moveToCurrentState;

		private Transform m_currentEntrance;
	
		private float m_currentWait;

		private enum EJobInWait
		{
			PowerSocket = 1,
			InteractWithSwitch = 2,
			PowerSocketAndInteractWithSwitch = 3
		}

		private enum EMoveToState
		{
			MoveToEndLocation = 1 << 0,
			Interact = 1 << 1,
			MoveToBlockingSystemEntrance = 1 << 2,
			MoveToBlockingSystemExit = 1 << 3,
			WaitForJobToComplete = 1 << 4
		}

		public override void OnStart()
		{
			base.OnStart();

			alreadyAtLocation = (location.Value - (Vector2)transform.position).sqrMagnitude < .5f;

			m_moveToCurrentState = EMoveToState.MoveToEndLocation;
			onBlockingElementOpened.RemoveAllListeners();
			OnPowerJobCompleted.RemoveAllListeners();
			reachedLocation = false;
			m_interactable = null;
			m_movingPoweredSystemOnPath = null;
			m_currentEntrance = null;

			if (path != null)
			{
				path.Release(this);
				path = null;
			}

			RequestPathToEndLocation();
		}

		public override void OnFixedUpdate()
		{
			base.OnFixedUpdate();
		}

		public override TaskStatus OnUpdate()
		{
			if (alreadyAtLocation) return TaskStatus.Success;
			if (pathImpossible) return TaskStatus.Failure;
			if (calculatingPath) return TaskStatus.Running;

			if (currentPathGotBlocked)
			{
				RequestPathToEndLocation();
				currentPathGotBlocked = false;
				return TaskStatus.Running;
			}
			
			switch (m_moveToCurrentState)
			{
				case EMoveToState.MoveToEndLocation:
					return reachedLocation ? TaskStatus.Success : TaskStatus.Running;

				case EMoveToState.Interact:
					return !reachedLocation ? TaskStatus.Running : ManageInteraction();

				case EMoveToState.MoveToBlockingSystemEntrance:
					return !reachedLocation ? TaskStatus.Running : ManageEntranceReachedBehavior();

				case EMoveToState.MoveToBlockingSystemExit:
					return !reachedLocation ? TaskStatus.Running : ManageExitReachedBehavior();
				
				case EMoveToState.WaitForJobToComplete:
					return ManageWaitForJobCompleteBehavior();

				default:
					return TaskStatus.Running;
			}
		}

		private TaskStatus ManageWaitForJobCompleteBehavior()
		{
			switch (m_jobsPending)
			{
				case EJobInWait.PowerSocket:
					return !m_movingPoweredSystemOnPath.Powered ? TaskStatus.Running : PowerSocketJobCompleted();

				case EJobInWait.InteractWithSwitch:
					return m_movingPoweredSystemOnPath.ControlledElementState != EControlledElementState.AlteredState
						? TaskStatus.Running : InteractionJobCompleted();

				case EJobInWait.PowerSocketAndInteractWithSwitch:
					return IsPowerAndInteractionJobComplete() ? TaskStatus.Running : PowerAndInteractionJobComplete();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private TaskStatus ManageEntranceReachedBehavior()
		{
			if (m_movingPoweredSystemOnPath.Powered && IsControllingSwitchAccessibleByUnit(m_movingPoweredSystemOnPath))
			{
				RequestPathToSwitch(m_interactable, EControlledElementState.AlteredState);
				return TaskStatus.Running;
			}

			if (!m_movingPoweredSystemOnPath.Powered && IsControllingSwitchAccessibleByUnit(m_movingPoweredSystemOnPath))
			{
				return TryAssigningPowerJob();
			}

			if (m_movingPoweredSystemOnPath.Powered && !IsControllingSwitchAccessibleByUnit(m_movingPoweredSystemOnPath))
			{
				return TryAssigningInteractionJob();
			}

			if (!m_movingPoweredSystemOnPath.Powered && !IsControllingSwitchAccessibleByUnit(m_movingPoweredSystemOnPath))
			{
				return TryAssigningPowerAndInteractionJob();
			}

			return TaskStatus.Running;
		}

		private bool IsPowerAndInteractionJobComplete()
		{
			return !m_movingPoweredSystemOnPath.Powered || m_movingPoweredSystemOnPath.ControlledElementState !=
				EControlledElementState.AlteredState;
		}

		private TaskStatus PowerAndInteractionJobComplete()
		{
			onBlockingElementOpened.Invoke();
			RequestPathToBlockingElementExit();
			return TaskStatus.Running;
		}

		private TaskStatus InteractionJobCompleted()
		{
			onBlockingElementOpened.Invoke();
			RequestPathToBlockingElementExit();
			return TaskStatus.Running;
		}

		private TaskStatus PowerSocketJobCompleted()
		{
			RequestPathToSwitch(m_interactable, EControlledElementState.AlteredState);
			return TaskStatus.Running;
		}

		private TaskStatus TryAssigningPowerAndInteractionJob()
		{
			var poweringUnit = m_unitAIController.CurrentArea.RequestUnitForJob(m_unitAIController, EUnitType.MaintenanceUnit,
				m_movingPoweredSystemOnPath
					.EnergySource.transform.position);
			UnitAIController interactingUnit = null;
			foreach (var s in m_movingPoweredSystemOnPath.ControlledSystemInteractables)
			{
				interactingUnit =
					m_unitAIController.CurrentArea.RequestUnitForJob(m_unitAIController, EUnitType.Sentry,
						s.transform.position);
				if (interactingUnit) break;
			}

			if (!poweringUnit || !interactingUnit) return TaskStatus.Failure;
			poweringUnit.AssignJob(new PowerSocketJob((EnergySocket)m_movingPoweredSystemOnPath.EnergySource,
				m_unitAIController.OnJobCompleted));
			interactingUnit.AssignJob(new MonitorControlledElementJob(m_movingPoweredSystemOnPath,
				EControlledElementState.AlteredState));
			m_jobsPending = EJobInWait.PowerSocketAndInteractWithSwitch;
			m_moveToCurrentState = EMoveToState.WaitForJobToComplete;
			return TaskStatus.Running;
		}

		private TaskStatus TryAssigningInteractionJob()
		{
			UnitAIController unit = null;
			foreach (var s in m_movingPoweredSystemOnPath.ControlledSystemInteractables)
			{
				unit = m_unitAIController.CurrentArea.RequestUnitForJob(m_unitAIController, EUnitType.Sentry,
					s.transform.position);
				if (unit) break;
			}

			if (!unit) return TaskStatus.Failure;

			unit.AssignJob(new MonitorControlledElementJob(m_movingPoweredSystemOnPath, EControlledElementState.AlteredState));
			m_jobsPending = EJobInWait.InteractWithSwitch;
			m_moveToCurrentState = EMoveToState.WaitForJobToComplete;
			return TaskStatus.Running;
		}

		private TaskStatus TryAssigningPowerJob()
		{
			var unit = m_unitAIController.CurrentArea.RequestUnitForJob(m_unitAIController, EUnitType.MaintenanceUnit,
				m_movingPoweredSystemOnPath.EnergySource.transform.position);

			if (!unit) return TaskStatus.Failure;
			unit.AssignJob(new PowerSocketJob((EnergySocket)m_movingPoweredSystemOnPath.EnergySource, m_unitAIController
				.OnJobCompleted));
			m_jobsPending = EJobInWait.PowerSocket;
			m_moveToCurrentState = EMoveToState.WaitForJobToComplete;
			return TaskStatus.Running;
		}

		private TaskStatus ManageInteraction()
		{
			if (m_unitAIController.UnitInteracter.InteractablesAvailable == null || !m_movingPoweredSystemOnPath.Powered)
			{
				RequestPathToEndLocation();
				return TaskStatus.Running;
			}

			if (m_movingPoweredSystemOnPath.IsTransitioning)
			{
				return TaskStatus.Running;
			}

			if (m_movingPoweredSystemOnPath.ControlledElementState != m_controlledElementDesiredState)
			{
				m_unitAIController.UnitInteracter.TryInteraction();
				return TaskStatus.Running;
			}

			if (m_movingPoweredSystemOnPath.ControlledElementState == m_controlledElementDesiredState)
			{
				if (m_controlledElementDesiredState == EControlledElementState.DefaultState)
				{
					OnPowerJobCompleted.Invoke();
					RequestPathToEndLocation();
				}

				else
				{
					RequestPathToBlockingElementExit();
				}
			}

			return TaskStatus.Running;
		}

		private TaskStatus ManageExitReachedBehavior()
		{
			if (m_movingPoweredSystemOnPath.mustBeClosedAfterPassing)
			{
				if (m_movingPoweredSystemOnPath.waitBeforeClosing)
				{
					m_currentWait += Time.deltaTime;
					if (m_currentWait <= m_movingPoweredSystemOnPath.closingDelay)
					{
						return TaskStatus.Running;
					}
					
					m_currentWait = 0;
				}
				
				var interactable = PathfindingUtilities.FindAccessibleInteractable(m_movingPoweredSystemOnPath, transform.position,
					GraphMask.FromGraphName("AreaGraph"));
				RequestPathToSwitch(interactable, EControlledElementState.DefaultState);
			}

			else
			{
				OnPowerJobCompleted.Invoke();
				RequestPathToEndLocation();
			}
			
			return TaskStatus.Running;
		}

		private void RequestPathToEndLocation()
		{
			endLocationToMoveTo = location.Value;
			m_pathGraphMask = GraphMask.FromGraphName("MainGraph");
			m_unitAIController.Seeker.graphMask = m_pathGraphMask;

			m_unitAIController.Seeker.StartPath(transform.position, endLocationToMoveTo, PathToEndLocationFound);
			calculatingPath = true;
		}
	
		private void RequestPathToSwitch(MovingPoweredSystemInteractable i, EControlledElementState desiredState)
		{
			m_controlledElementDesiredState = desiredState;
			Vector2 switchLocation = i.transform.position;
			m_pathGraphMask = GraphMask.FromGraphName("MainGraph");
			m_unitAIController.Seeker.graphMask = m_pathGraphMask;
			m_unitAIController.Seeker.StartPath(transform.position, switchLocation, PathToSwitchFound);
			calculatingPath = true;
		}

		private void RequestPathToBlockingElementEntrance(PathBlockingMovingPoweredSystem blockingPoweredSystem)
		{
			m_currentEntrance = FindAccessibleEntrance(blockingPoweredSystem);

			if (m_currentEntrance == null)
			{
				pathImpossible = true;
				return;
			}
			
			m_unitAIController.Seeker.StartPath(transform.position, m_currentEntrance.position, PathToControlledElementEntranceFound);
			calculatingPath = true;
		}

		private Transform FindAccessibleEntrance(PathBlockingMovingPoweredSystem blockingSystem)
		{
			var nn = new NNConstraint
			{
				constrainWalkability = true,
				walkable = true,
				graphMask = GraphMask.FromGraphName("AreaGraph"),
			};
		
			var unitNode = AstarPath.active.GetNearest(m_unitAIController.transform.position, nn).node;
			
			foreach (var entrance in blockingSystem.elementEntrances)
			{
				var entranceNode = AstarPath.active.GetNearest(entrance.position, nn).node;
				if (unitNode.Area != entranceNode.Area) continue;
				return entrance;
			}

			return null;
		}

		private void RequestPathToBlockingElementExit()
		{
			Transform exitTransform = m_movingPoweredSystemOnPath.elementEntrances.First(x => x != m_currentEntrance);
			m_unitAIController.Seeker.StartPath(transform.position, exitTransform.position, PathToControlledElementExitFound);
			calculatingPath = true;
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

					m_moveToCurrentState = EMoveToState.MoveToEndLocation;
					currentWaypoint = 1;
					reachedLocation = false;
					followingPath = true;
					currentPathGotBlocked = false;
					return;
				}

				RequestPathToBlockingElementEntrance(m_movingPoweredSystemOnPath);
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
			
				m_moveToCurrentState = EMoveToState.Interact;
				reachedLocation = false;
				followingPath = true;
				currentPathGotBlocked = false;
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
			
				m_moveToCurrentState = EMoveToState.MoveToBlockingSystemEntrance;
				reachedLocation = false;
				followingPath = true;
				currentPathGotBlocked = false;
				return;
			}

			pathImpossible = true;
		}

		private void PathToControlledElementExitFound(Path p)
		{
			calculatingPath = false;
		
			if (!p.error)
			{
				if (path != null) path.Release(this);
				m_unitAIController.Seeker.drawGizmos = true;
			
				path = p;
				p.Claim(this);
				currentWaypoint = 1;
			
				m_moveToCurrentState = EMoveToState.MoveToBlockingSystemExit;
				reachedLocation = false;
				followingPath = true;
				currentPathGotBlocked = false;
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
			foreach (var controlledSystemInteractable in movingPoweredSystem.ControlledSystemInteractables)
			{
				var n1 = m_areaGraph.GetNearest(AIController.Value.transform.position, NNConstraint.Default).node;
				var n2 = m_areaGraph.GetNearest(controlledSystemInteractable.transform.position, NNConstraint.Default).node;

				if (n1.Area != n2.Area) continue;
				m_interactable = controlledSystemInteractable;
				return true;
			}

			return false;
		}

		public override void OnConditionalAbort()
		{
			base.OnConditionalAbort();
			m_currentWait = 0;
		}
	}
}
