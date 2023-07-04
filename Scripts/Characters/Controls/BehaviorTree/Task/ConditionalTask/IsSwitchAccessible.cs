using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;
using Gameplay.InteractionSystem.Interactables;
using Gameplay.PoweredObjects.ControlledPoweredObjects;
using Pathfinding;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask
{
	public class IsSwitchAccessible : Action
	{
		public SharedAIController AIController;

		private UnitAIController m_unitAIController;

		public SharedVector2 location;
	
		private GridGraph areaGraph;
		private GridGraph mainGraph;
	
		private GraphMask m_pathGraphMask;

		public LayerMask dynamicObstacleLayerMask;

		private bool m_pathFound = false;
		private bool m_failedToFindPath = false;
		private Path path;

		private List<GraphNode> m_alteredNodes = new List<GraphNode>();
	
		public override void OnAwake()
		{
			base.OnAwake();
		
			m_unitAIController = (UnitAIController) AIController.Value;
			areaGraph = (GridGraph) AstarPath.active.data.graphs[1];
			mainGraph = AstarPath.active.data.gridGraph;
		}

		public override void OnStart()
		{
			base.OnStart();
			m_pathFound = false;
			RequestPath();
		}

		public override TaskStatus OnUpdate()
		{
			if (m_failedToFindPath)
			{
				if (m_alteredNodes.Count == 0) return TaskStatus.Failure;
			
				foreach (var alteredNode in m_alteredNodes)
				{
					alteredNode.Walkable = true;
				}
			
				return TaskStatus.Failure;
			}
		
			if (!m_pathFound)
			{
				return TaskStatus.Running;
			}

			MovingPoweredSystem blockingPoweredSystem = FindBlockingElement();

			if (!blockingPoweredSystem) return TaskStatus.Failure;

			MovingPoweredSystemInteractable s = FindAccessibleSwitch(blockingPoweredSystem);

			if (s)
			{
				location.Value = s.transform.position;
			
				if (m_alteredNodes.Count > 0)
				{
					foreach (var alteredNode in m_alteredNodes)
					{
						alteredNode.Walkable = true;
					}
				}
				
				return TaskStatus.Success;
			}
		
			GraphNode obstacleNode = mainGraph.GetNearest(blockingPoweredSystem.transform.position, NNConstraint.Default).node;
				
			var connections = new List<GraphNode>();
			obstacleNode.GetConnections(connections.Add);

			foreach (var connection in connections)
			{
				if (connection.Walkable)
				{
					connection.Walkable = false;
					if (!m_alteredNodes.Contains(connection))
					{
						m_alteredNodes.Add(connection);
					}
				}
			}

			obstacleNode.Walkable = false;
			m_alteredNodes.Add(obstacleNode);
			RequestPath();
			return TaskStatus.Running;
		}

		private MovingPoweredSystem FindBlockingElement()
		{
			foreach (var node in path.path)
			{
				Collider2D obstacle = Physics2D.OverlapCircle((Vector3) node.position, .5f, dynamicObstacleLayerMask);
				if (!obstacle) continue;
			
				return obstacle.GetComponent<MovingPoweredSystem>();
			}

			return null;
		}

		private MovingPoweredSystemInteractable FindAccessibleSwitch(MovingPoweredSystem movingPoweredSystem)
		{
			if (!movingPoweredSystem.Powered) return null;
		
			foreach (var s in movingPoweredSystem.ControlledSystemInteractables)
			{
				var n1 = areaGraph.GetNearest(AIController.Value.transform.position, NNConstraint.Default).node;
				var n2 = areaGraph.GetNearest(s.transform.position, NNConstraint.Default).node;

				if (n1.Area == n2.Area)
				{
					return s;
				}
			}

			return null;
		}
	
		private void RequestPath()
		{
			m_pathFound = false;
			m_pathGraphMask = GraphMask.FromGraphName("MainGraph");
			m_unitAIController.Seeker.graphMask = m_pathGraphMask;
			m_unitAIController.Seeker.StartPath(transform.position, (Vector3)location.Value, PathFound);
		}

		public void PathFound(Path p)
		{
			if (p.error)
			{
				m_failedToFindPath = true;
			}
		
			path = p;
			m_pathFound = true;
		}
	}
}