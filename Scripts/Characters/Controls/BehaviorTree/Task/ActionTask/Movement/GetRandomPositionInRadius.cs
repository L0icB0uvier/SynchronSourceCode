using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Movement
{
	[TaskCategory("Movement")]
	public class GetRandomPositionInRadius : Action
	{
		public SharedAIController AIController;
		public SharedVector2 randomPosition;
		public int minRange;
		public int maxRange;

		public override void OnStart()
		{
			Vector2 randomPos = Random.insideUnitCircle;
			randomPos *= Random.Range(minRange, maxRange);
			Vector2 pos2D = AIController.Value.transform.position;
			randomPos = pos2D + randomPos;


			randomPosition.Value = randomPos;
		}

		public override TaskStatus OnUpdate()
		{
			return TaskStatus.Success;
		}
	}
}