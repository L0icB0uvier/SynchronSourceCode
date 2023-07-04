using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Pathfinding;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask
{
	public class CanReachLocation : Conditional
	{
		public SharedAIController AIController;

		public SharedVector2 location;

		private GridGraph mainGraph;
	
		public override void OnAwake()
		{
			base.OnAwake();
			mainGraph = AstarPath.active.data.gridGraph;
		}

		public override TaskStatus OnUpdate()
		{
			GraphNode n1;
			GraphNode n2;
		
			n1 = mainGraph.GetNearest(AIController.Value.transform.position, NNConstraint.Default).node;
			n2 = mainGraph.GetNearest(location.Value, NNConstraint.Default).node;

			return n1.Area == n2.Area ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}