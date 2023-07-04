using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Movement
{
	public class GetObjectLocation : Action
	{
		public SharedGameObject Target;

		public SharedVector2 objectLocation;
	
		public override TaskStatus OnUpdate()
		{
			objectLocation.Value = Target.Value.transform.position;
			return TaskStatus.Running;
		}
	}
}