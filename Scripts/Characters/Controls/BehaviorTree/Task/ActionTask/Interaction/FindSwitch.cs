using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies.Units.ControllerImplementation;
using Gameplay.PoweredObjects.ControlledPoweredObjects;
using Pathfinding;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.Interaction
{
	public class FindSwitch : Action
	{
		public SharedAIController AIController;

		private SentryAIController m_sentryAIController;

		public SharedVector2 SwitchLocation;

		public SharedControlledElement MonitoredControlledElement;

		private GridGraph m_mainGraph;
		private GridGraph m_areaGraph;

		private List<Vector3> m_reachableSwitchLocations = new List<Vector3>();
		private List<Vector3> m_sameAreaSwitchLocations = new List<Vector3>();

		private bool m_waitForPathCalculation;
		private bool m_pathCalculationSucceeded;
		private bool m_pathCalculationFailed;
	
		public override void OnAwake()
		{
			base.OnAwake();
			m_sentryAIController = (SentryAIController) AIController.Value;
			m_mainGraph = AstarPath.active.data.gridGraph;
			m_areaGraph = (GridGraph) AstarPath.active.data.graphs[1];
		}

		public override void OnStart()
		{
			base.OnStart();
			m_waitForPathCalculation = false;
			m_pathCalculationSucceeded = false;
			m_pathCalculationFailed = false;
			m_reachableSwitchLocations.Clear();
			m_sameAreaSwitchLocations.Clear();
		}

		public override TaskStatus OnUpdate()
		{
			if (m_pathCalculationSucceeded) return TaskStatus.Success;
			if (m_pathCalculationFailed) return TaskStatus.Failure;
			if (m_waitForPathCalculation) return TaskStatus.Running;

			foreach (var s in MonitoredControlledElement.Value.ControlledSystemInteractables)
			{
				GraphNode n1 = m_mainGraph.GetNearest(AIController.Value.transform.position, NNConstraint.Default).node;
				GraphNode n2 = m_mainGraph.GetNearest(s.transform.position, NNConstraint.Default).node;
				if(n1.Area != n2.Area) continue;
			
				m_reachableSwitchLocations.Add(s.transform.position);
			
				n1 = m_areaGraph.GetNearest(AIController.Value.transform.position, NNConstraint.Default).node;
				n2 = m_areaGraph.GetNearest(s.transform.position, NNConstraint.Default).node;
				if (n1.Area != n2.Area) continue;
			
				m_sameAreaSwitchLocations.Add(s.transform.position);
			}

			if (m_reachableSwitchLocations.Count == 0) return TaskStatus.Failure;

			if (m_sameAreaSwitchLocations.Count > 0)
			{
				if (m_sameAreaSwitchLocations.Count == 1)
				{
					SwitchLocation.Value = m_sameAreaSwitchLocations[0];
					return TaskStatus.Success;
				}
			
				m_sentryAIController.Seeker.StartMultiTargetPath(transform.position, m_sameAreaSwitchLocations.ToArray(), false, 
					Callback, 2);
				m_waitForPathCalculation = true;
				return TaskStatus.Running;
			}

			if (m_reachableSwitchLocations.Count == 1)
			{
				SwitchLocation.Value = m_reachableSwitchLocations[0];
				return TaskStatus.Success;
			}
		
			m_sentryAIController.Seeker.StartMultiTargetPath(transform.position, m_reachableSwitchLocations.ToArray(), 
				true, Callback, 1);
			m_waitForPathCalculation = true;
			return TaskStatus.Running;
		}
	
		private void Callback(Path p)
		{
			if (p.error) {
				Debug.Log("Ouch, the path returned an error\nError: "+p.errorLog);
				m_pathCalculationFailed = true;
				return;
			}
		
			MultiTargetPath mp = p as MultiTargetPath;
			Debug.Log(mp.originalTargetPoints[mp.chosenTarget]);
		
			SwitchLocation.Value = mp.originalTargetPoints[mp.chosenTarget];
			m_pathCalculationSucceeded = true;
		}
	}
}