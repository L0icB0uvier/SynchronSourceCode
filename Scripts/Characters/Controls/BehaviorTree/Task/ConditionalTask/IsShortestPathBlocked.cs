using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;
using Pathfinding;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask
{
	public class IsShortestPathBlocked : Conditional
	{
		public SharedAIController AIController;

		private UnitAIController m_unitAIController;
	
		public SharedVector2 location;
	
		private GridGraph mainGraph;
		private GridGraph areaGraph;

		private bool pathsCalculated;
	
		private GraphMask m_pathGraphMask;

		private Path m_mainPath;
		private Path m_areaPath;
	
		private enum EGraphType{MainGraph, AreaGraph}

		public float mainPathLenght;
		public float areaPathLenght;

		private Vector2 m_previousLocation;

	
		public bool dynamic;

		public float recalculateThreshold = 1;

		public override void OnAwake()
		{
			base.OnAwake();
			m_unitAIController = (UnitAIController) AIController.Value;
			mainGraph = AstarPath.active.data.gridGraph;
			areaGraph = (GridGraph) AstarPath.active.data.graphs[1];
		}

		public override void OnStart()
		{
			base.OnStart();
			pathsCalculated = false;
			m_previousLocation = location.Value;
			// RequestPath(MoveTo.EGraphType.MainGraph);
		}

		public override TaskStatus OnUpdate()
		{
			if (dynamic && pathsCalculated && (location.Value - m_previousLocation).sqrMagnitude > recalculateThreshold)
			{
				pathsCalculated = false;
				m_previousLocation = location.Value;
				// RequestPath(MoveTo.EGraphType.MainGraph);
			}
		
			if (!pathsCalculated) return TaskStatus.Running;

			mainPathLenght = m_mainPath.GetTotalLength();
			areaPathLenght = m_areaPath.GetTotalLength();
			return Math.Abs(m_mainPath.GetTotalLength() - m_areaPath.GetTotalLength()) < 1 ? TaskStatus.Failure : TaskStatus.Success;
		}
	
		// private void RequestPath(MoveTo.EGraphType graphType)
		// {
		// 	m_pathGraphMask = GraphMask.FromGraphName(graphType.ToString());
		// 	m_unitAIController.Seeker.graphMask = m_pathGraphMask;
		//
		// 	switch (graphType)
		// 	{
		// 		case MoveTo.EGraphType.MainGraph:
		// 			m_unitAIController.Seeker.StartPath(transform.position, (Vector3)location.Value, MainGraphPathFound);
		// 			break;
		// 		case MoveTo.EGraphType.AreaGraph:
		// 			m_unitAIController.Seeker.StartPath(transform.position, (Vector3)location.Value, AreaGraphPathFound);
		// 			break;
		// 	}
		// }
		//
		// private void MainGraphPathFound(Path p)
		// {
		// 	m_mainPath = p;
		// 	RequestPath(MoveTo.EGraphType.AreaGraph);
		// }
		//
		// private void AreaGraphPathFound(Path p)
		// {
		// 	m_areaPath = p;
		// 	pathsCalculated = true;
		// }
	}
}

