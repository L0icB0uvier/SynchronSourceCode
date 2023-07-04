using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ActionTask.IntrusionTasks.Search
{
	public class DefineSearchPerimeter : Action
	{
		public SharedVector2 searchOrigin;

		private List<Vector2> m_searchLocations = new List<Vector2>();
		public List<Vector2> SearchLocations => m_searchLocations;

		public override void OnStart()
		{
			base.OnStart();
			m_searchLocations.Clear();
		}

		public override TaskStatus OnUpdate()
		{
			searchOrigin.Value = transform.position;
			return TaskStatus.Success;
		}

		public void AddSearchLocation(Vector2 location)
		{
			m_searchLocations.Add(location);
		}
	}
}