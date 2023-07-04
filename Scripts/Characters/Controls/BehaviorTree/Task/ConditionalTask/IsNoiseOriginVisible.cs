using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask
{
	public class IsNoiseOriginVisible : Conditional
	{
		public SharedAIController AIController;

		public SharedVector2 noiseOrigin;

		public LayerMask obstaclesLayerMask;

		public override TaskStatus OnUpdate()
		{
			if (Physics2D.Linecast(AIController.Value.transform.position, noiseOrigin.Value, obstaclesLayerMask.value))
			{
				return TaskStatus.Failure;
			}

			else
			{
				return TaskStatus.Success;
			}
		}
	}
}