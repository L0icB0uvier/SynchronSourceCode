using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using GeneralScriptableObjects;
using Pathfinding;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.IntrusionTasks
{
	public class FindRandomLocation : Action
	{
		public SharedAIController AIController;
		private GridGraph m_areaGridGraph;

		public SharedVector2 RandomLocation;

		public FloatVariable radius;
		private bool m_foundReachablePoint = false;
		private GraphNode m_posNode;

		public override void OnAwake()
		{
			
			base.OnAwake();
			m_areaGridGraph = (GridGraph) AstarPath.active.data.graphs[1];
		}

		public override void OnStart()
		{
			m_posNode = m_areaGridGraph.GetNearest(AIController.Value.transform.position, NNConstraint.Default).node;
		}

		public override TaskStatus OnUpdate()
		{
			while (!m_foundReachablePoint)
			{
				Vector3 point = (Vector2)AIController.Value.transform.position + (Random.insideUnitCircle * radius.Value);
				GraphNode n = m_areaGridGraph.GetNearest(point, NNConstraint.Default).node;
				if (n.Walkable && n.Area == m_posNode.Area)
				{
					RandomLocation.Value = (Vector3) n.position;
					m_foundReachablePoint = true;
				}
			}
		
			return TaskStatus.Success;
		}
	}
}