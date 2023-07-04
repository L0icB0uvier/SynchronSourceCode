using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.BehaviorTree.SharedVariables;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies;
using Pathfinding;
using SceneManagement.LevelManagement;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask
{
	[TaskCategory("Queries")]
	public class FindBestTarget : Action
	{
		public SharedAIController AIController;
		private EnemyAIController m_enemyAIController;
		private AreaManager m_area;
		public SharedTargetInfo TargetInfo;
		private GridGraph m_mainGridGraph;

		private GraphNode n1;
		private GraphNode n2;
	
		public override void OnAwake()
		{
			base.OnAwake();
			m_enemyAIController = (EnemyAIController)AIController.Value;
			m_area = m_enemyAIController.CurrentArea;
			m_mainGridGraph = AstarPath.active.data.gridGraph;
		}

		public override TaskStatus OnUpdate()
		{
			if (m_area.Targets.Count == 1)
			{
				n1 = m_mainGridGraph.GetNearest(AIController.Value.transform.position,NNConstraint.Default).node;
				n2 = m_mainGridGraph.GetNearest(m_area.Targets[0].lastKnownLocation,NNConstraint.Default).node;
			
				if (!PathUtilities.IsPathPossible(n1, n2)) return TaskStatus.Failure;
			
				TargetInfo.Value = m_area.Targets[0];
				return TaskStatus.Success;
			}

			List<TargetInfo> reachableTargets = new List<TargetInfo>();
		
			n1 = m_mainGridGraph.GetNearest(AIController.Value.transform.position,NNConstraint.Default).node;

			foreach (var target in m_area.Targets)
			{
				n2 = m_mainGridGraph.GetNearest(target.lastKnownLocation,NNConstraint.Default).node;
			
				if (!PathUtilities.IsPathPossible(n1, n2)) continue;
			
				reachableTargets.Add(target);
			}

			switch (reachableTargets.Count)
			{
				case 0:
					return TaskStatus.Failure;
				case 1:
					TargetInfo.Value = reachableTargets[0];
					return TaskStatus.Success;
				default:
					reachableTargets.OrderBy(x =>
						(x.lastKnownLocation - (Vector2) AIController.Value.transform.position).sqrMagnitude);
					List<TargetInfo> visibleTargets = reachableTargets.Where(target => target.currentlyVisible).ToList();

					switch (visibleTargets.Count)
					{
						case 0:
							TargetInfo.Value = reachableTargets[0];
							return TaskStatus.Success;
						case 1:
							TargetInfo.Value = visibleTargets[0];
							return TaskStatus.Success;
						default:
							visibleTargets.OrderBy(x =>
								(x.lastKnownLocation - (Vector2) AIController.Value.transform.position).sqrMagnitude);
							TargetInfo.Value = visibleTargets[0];
							return TaskStatus.Success;
					}
			}
		}
	}
}