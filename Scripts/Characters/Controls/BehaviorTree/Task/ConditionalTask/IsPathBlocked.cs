using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Pathfinding;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask
{
	public class IsPathBlocked : Conditional
	{
		public SharedAIController AIController;

		public SharedVector2 location;

		private GridGraph areaGraph;
	
		public override void OnAwake()
		{
			base.OnAwake();
			areaGraph = (GridGraph) AstarPath.active.data.graphs[1];
		}

		public override TaskStatus OnUpdate()
		{
			GraphNode n1;
			GraphNode n2;
		
			n1 = areaGraph.GetNearest(AIController.Value.transform.position, NNConstraint.Default).node;
			n2 = areaGraph.GetNearest(location.Value, NNConstraint.Default).node;
		
			return n1.Area != n2.Area ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}