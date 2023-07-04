using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;
using Pathfinding;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask.Behavior
{
	public class IsPatrolPointsReachable : Conditional
	{
		public SharedAIController AIController;

		private UnitAIController m_unitAIController;

		private GridGraph mainGraph;
		// GridGraph AreaGraph;

		public override void OnAwake()
		{
			base.OnAwake();
			m_unitAIController = (UnitAIController) AIController.Value;
		
			mainGraph = AstarPath.active.data.gridGraph;
			// AreaGraph = (GridGraph)AstarPath.active.data.graphs[1];
		}

		public override TaskStatus OnUpdate()
		{
			var reachablePpCount = 0;

			foreach (var patrolPoint in m_unitAIController.CurrentPathPatrolPoints)
			{
				var node1 = mainGraph.GetNearest(patrolPoint.patrolPoint.transform.position, NNConstraint.Default).node;
				var node2 = mainGraph.GetNearest(m_unitAIController.transform.position, NNConstraint.Default)
					.node;

				if (node1.Area == node2.Area)
				{
					reachablePpCount++;
				}
			}
	
			return reachablePpCount < 2 ? TaskStatus.Failure : TaskStatus.Success;
		}
	}
}