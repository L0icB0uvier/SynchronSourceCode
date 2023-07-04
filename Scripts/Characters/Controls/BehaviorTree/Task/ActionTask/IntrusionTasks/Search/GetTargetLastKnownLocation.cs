using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.BehaviorTree.SharedVariables;
using Pathfinding;
using SceneManagement.LevelManagement;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.IntrusionTasks.Search
{
	[TaskCategory("Behavior/Search")]
	public class GetTargetLastKnownLocation : Action
	{
		public SharedVector2 targetLastKnownLocation;
		public SharedTargetInfo target;

		public float locationExtrapolation = 2;

		public override TaskStatus OnUpdate()
		{
			if (target.Value == null) return TaskStatus.Failure;
			Vector2 extrapolatedLocation = target.Value.lastKnownLocation + target.Value.targetMovingDirection * locationExtrapolation;

			GraphNode node = AstarPath.active.data.gridGraph.GetNearest(extrapolatedLocation, NNConstraint.Default).node;
			if (node.Walkable)
			{
				targetLastKnownLocation.Value = extrapolatedLocation;
				return TaskStatus.Success;
			}
		
			var connections = new List<GraphNode>();
			node.GetConnections(connections.Add);

			foreach (var graphNode in connections)
			{
				if (graphNode.Walkable)
				{
					targetLastKnownLocation.Value = (Vector3) graphNode.position;
					return TaskStatus.Success;
				}
			}
		
			return TaskStatus.Failure;
		}
	}
}