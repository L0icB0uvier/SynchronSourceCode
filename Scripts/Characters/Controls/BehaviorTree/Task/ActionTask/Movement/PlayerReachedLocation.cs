using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.PlayerControllers.Hicks;
using GeneralScriptableObjects.Events;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Movement
{
	[TaskCategory("Movement/Move")]
	public class PlayerReachedLocation : Action
	{
		public VoidEventChannelSO onLocationReachedChannel;

		public override TaskStatus OnUpdate()
		{
			onLocationReachedChannel.RaiseEvent();
			return TaskStatus.Success;
		}
	}
}