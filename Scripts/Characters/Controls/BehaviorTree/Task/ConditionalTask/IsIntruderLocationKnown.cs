using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.BehaviorTree.SharedVariables;
using SceneManagement.LevelManagement;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask
{
	[TaskCategory("Search")]
	public class IsIntruderLocationKnown : Conditional
	{
		public SharedTargetInfo target;

		public SharedGameObject targetLocation;


		public override TaskStatus OnUpdate()
		{
			if (target.Value == null || !target.Value.currentlyVisible) return TaskStatus.Failure;
		
			targetLocation.Value = target.Value.targetController.gameObject;
			return TaskStatus.Success;
		}
	}
}