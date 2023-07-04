using BehaviorDesigner.Runtime.Tasks;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Jobs
{
	public class PowerCellSearchCompleted : Action
	{
		public SharedEnergySocket Socket;

		public override TaskStatus OnUpdate()
		{
			Socket.Value.monitoredState = Socket.Value.socketState;
			return TaskStatus.Success;
		}
	}
}