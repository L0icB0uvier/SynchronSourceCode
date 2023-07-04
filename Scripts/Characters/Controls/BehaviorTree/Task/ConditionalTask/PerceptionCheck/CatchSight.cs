using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Enemies.Perception;
using Pathfinding;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask.PerceptionCheck
{
	[TaskCategory("Perception")]
	public class CatchSight : Conditional
	{
		public SharedAIController AIController;
	
		FieldOfView fov;
	
		public SharedVector2 AnomalyLocation;

		public override void OnAwake()
		{
			base.OnAwake();
			fov = AIController.Value.transform.GetComponentInChildren<FieldOfView>();
		}

		public override TaskStatus OnUpdate()
		{
			if (!fov.SeeingSomething) return TaskStatus.Failure;
		
			AnomalyLocation.Value = (Vector3)PathfindingUtilities.GetNearestNavigableNode(fov.AnomalyLocation, GraphMask
				.FromGraphName("MainGraph")).position ;
			return TaskStatus.Success;
		}
	}
}