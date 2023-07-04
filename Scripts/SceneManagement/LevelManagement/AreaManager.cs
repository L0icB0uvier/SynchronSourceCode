using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters.Controls.Controllers.AIControllers.Enemies.Units;
using Characters.Controls.Controllers.PlayerControllers;
using GeneralScriptableObjects.Events;
using Pathfinding;
using Sirenix.OdinInspector;
using Tools.Extension;
using UnityEngine;

namespace SceneManagement.LevelManagement
{
	[Serializable]
	[LabelText("Area")]
	public class AreaManager : MonoBehaviour
	{
		[SerializeField] private EAreaStatus areaStatus;
		public EAreaStatus AreaStatus => areaStatus;

		public List<TargetInfo> Targets { get; } = new List<TargetInfo>();

		[SerializeField][ReadOnly][FoldoutGroup("Debug")]
		private bool searchingArea;
		public bool SearchingArea => searchingArea;

		[SerializeField] private float searchDuration = 30;
		
		[Header("Broadcast on")]
		[SerializeField] private VoidEventChannelSO alertStartChannel;
		[SerializeField] private VoidEventChannelSO alertEndChannel;
		
		[Header("Listening to")]
		[SerializeField] private VoidEventChannelSO resetAreaManagerChannel;

		private List<UnitAIController> m_units = new List<UnitAIController>();
		private List<UnitAIController> m_unitsWithJobs = new List<UnitAIController>();

		[EnumFlags]
		public EUnitBehavior jobAcceptingBehavior;

		private GridGraph m_areaGraph;

		private Coroutine m_searchTimer;

		public void Awake()
		{
			InitializeArea();
		
			GetLevelEnemiesInfo();
		}

		private void Start()
		{
			m_areaGraph = (GridGraph) AstarPath.active.data.graphs[1];
		}

		private void OnEnable()
		{
			resetAreaManagerChannel.onEventRaised += InitializeArea;
		}

		private void OnDisable()
		{
			resetAreaManagerChannel.onEventRaised -= InitializeArea;
		}
		
		private void LateUpdate()
		{
			if (areaStatus != EAreaStatus.Alert) return;
			
			for (int i = Targets.Count -1; i >=0; i--)
			{
				if (Targets[i].currentlyVisible)
				{
					Targets[i].UpdateLastKnownLocation();
				}
				
				else if (Targets[i].lastLocationChecked)
				{
					Targets.RemoveAt(i);
				}
			}
			
			if (!searchingArea && !Targets.Any(x => x.currentlyVisible))
			{
				StartSearchInArea();
			}
		}
	
		[Button]
		public void GetLevelEnemiesInfo()
		{
			m_units = FindObjectsOfType<UnitAIController>().ToList();
		}
		
		public UnitAIController RequestUnitForJob(UnitAIController requestingUnit, EUnitType unitType, Vector3 jobLocation)
		{
			foreach (var unit in m_units)
			{
				unit.GetGraphAreas();
			}
		
			GraphNode jobNode = m_areaGraph.GetNearest(jobLocation, NNConstraint.Default).node;
		
			var availableUnits = m_units.Where(unit => unit.UnitType == unitType && (unit.UnitCurrentBehavior & 
				jobAcceptingBehavior) != 0 && unit != requestingUnit && !m_unitsWithJobs.Contains(unit)).ToArray();

			if (availableUnits.Length == 0)
			{
				print("No unit available for job");
				return null;
			}

			var availableUnitsInArea = availableUnits.Where(unit => unit.AreaGraphCurrentArea == jobNode.Area).ToArray();

			if (availableUnitsInArea.Length == 0)
			{
				return null;
			}

			if(availableUnitsInArea.Length > 1) availableUnitsInArea = availableUnitsInArea.OrderBy(x => (x.transform
					.position - jobLocation).sqrMagnitude)
				.ToArray();
		
			m_unitsWithJobs.Add(availableUnitsInArea[0]);
			return availableUnitsInArea[0];
		}

		public void UnitReportJobCompleted(UnitAIController unit)
		{
			if(m_unitsWithJobs.Contains(unit)) m_unitsWithJobs.Remove(unit);
		}

		public void UnitReportJobInterruption(UnitAIController unit)
		{
			if(m_unitsWithJobs.Contains(unit)) m_unitsWithJobs.Remove(unit);
		}

		public void IntruderVisualAcquired(PlayerController target, GameObject unit)
		{
			TargetInfo targetInfo = Targets.Find(x => x.targetController == target);

			if(targetInfo == null)
			{
				targetInfo = new TargetInfo(target);
				Targets.Add(targetInfo);
			}

			targetInfo.UnitGainVisual(unit);
		
			if(AreaStatus != EAreaStatus.Alert)
			{
				StartAlertInArea();
			}

			if (m_searchTimer != null)
			{
				StopAreaSearch();
			}
		}

		private void StopAreaSearch()
		{
			StopCoroutine(m_searchTimer);
			searchingArea = false;
			m_searchTimer = null;
		}

		public void IntruderVisualLost(PlayerController target, GameObject unit)
		{
			TargetInfo targetInfo = Targets.Find(x => x.targetController == target);

			targetInfo?.UnitLostVisual(unit);
		}

		public void StartSearchInArea()
		{
			searchingArea = true;
			m_searchTimer = StartCoroutine(SearchTimer());
		}

		private IEnumerator SearchTimer()
		{
			yield return new WaitForSecondsRealtime(searchDuration);
			ReturnToNormalStatus();
			StopAreaSearch();
		}
		
		public void InitializeArea()
		{
			ReturnToNormalStatus();

			Targets.Clear();
			m_unitsWithJobs.Clear();
		}

		public void ChangeAreaStatus(EAreaStatus newStatus)
		{
			areaStatus = newStatus;
		}

		public void ReturnToNormalStatus()
		{
			ChangeAreaStatus(EAreaStatus.Normal);

			alertEndChannel.RaiseEvent();
		}

		public void StartAlertInArea()
		{
			ChangeAreaStatus(EAreaStatus.Alert);
			alertStartChannel.RaiseEvent();
		}
	}

	public enum EAreaStatus {Normal, Alert, Vigilance}
}