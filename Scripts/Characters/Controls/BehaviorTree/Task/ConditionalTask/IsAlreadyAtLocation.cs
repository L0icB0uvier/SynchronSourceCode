using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask
{
	public class IsAlreadyAtLocation : Conditional
	{
		public SharedAIController AIController;

		public SharedVector2 location;

		public float distanceThreshold = 1;

		public override TaskStatus OnUpdate()
		{
			Vector2 pos2D = AIController.Value.transform.position;
			if ((pos2D - location.Value).sqrMagnitude < distanceThreshold * distanceThreshold)
			{
				return TaskStatus.Success;
			}

			else
			{
				return TaskStatus.Failure;
			}
		}
	}
}