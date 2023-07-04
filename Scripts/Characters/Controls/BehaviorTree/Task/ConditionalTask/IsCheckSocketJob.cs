using BehaviorDesigner.Runtime.Tasks;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask
{
	public class IsCheckSocketJob : Conditional
	{
		public override TaskStatus OnUpdate()
		{
			return TaskStatus.Success;
		}
	}
}