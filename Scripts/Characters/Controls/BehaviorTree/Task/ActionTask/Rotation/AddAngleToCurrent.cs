using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Rotation
{
	[TaskCategory("Movement/Rotation")]
	public class AddAngleToCurrent : Action
	{
		public SharedAIController AIController;
		public SharedInt AngleToLookAt;

		[Range(-180, 180)]
		public int modifiedBy;

		public override TaskStatus OnUpdate()
		{
			int newAngle = Mathf.RoundToInt(AIController.Value.LookingDirection) + modifiedBy;

			if (newAngle < 0)
				newAngle += 360;

			if (newAngle > 360)
				newAngle -= 360;

			AngleToLookAt.Value = newAngle;
			return TaskStatus.Success;
		}
	}
}