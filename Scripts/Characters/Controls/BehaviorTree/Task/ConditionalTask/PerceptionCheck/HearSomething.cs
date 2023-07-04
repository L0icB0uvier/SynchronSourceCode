using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using GeneralScriptableObjects;
using NoiseSystem;
using Pathfinding;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask.PerceptionCheck
{
	[TaskCategory("Perception")]
	public class HearSomething : Conditional
	{
		public SharedVector2 AnomalyLocation;

		private Collider2D[] m_noiseCollider = new Collider2D[5];

		public LayerMaskVariable noiseLayerMask;

		public LayerMaskVariable obstacleLayerMask;

		private List<Noise> m_noisesHeard = new List<Noise>();

		public override void OnStart()
		{
			base.OnStart();
		
			Array.Clear(m_noiseCollider, 0, m_noiseCollider.Length - 1);
		}

		public override TaskStatus OnUpdate()
		{
			if(Physics2D.OverlapPointNonAlloc(transform.position, m_noiseCollider, noiseLayerMask.Value) == 0) return TaskStatus.Failure;
		
			m_noisesHeard.Clear();
			foreach (var t in m_noiseCollider)
			{
				if (!t) continue;
				Noise noise = t.GetComponent<Noise>();
				if (noise.NoiseInstigator == ENoiseInstigator.Enemies || 
				    noise.StoppedByWalls && Physics2D.Linecast(transform.position, noise.transform.position, obstacleLayerMask.Value))
				{
					continue;
				}

				m_noisesHeard.Add(noise);
			}

			if (m_noisesHeard.Count == 0)
			{
				return TaskStatus.Failure;
			}

			if (m_noisesHeard.Count > 1)
			{
				m_noisesHeard = m_noisesHeard.OrderByDescending(x => x.EmissionTime).ToList();
			}

			Vector3 noiseGraphPosition = (Vector3)PathfindingUtilities.GetNearestNavigableNode(m_noisesHeard[0].transform.position, GraphMask.FromGraphName("MainGraph")).position;

			if (AnomalyLocation.Value == (Vector2)noiseGraphPosition) return TaskStatus.Failure;
			
			AnomalyLocation.Value = noiseGraphPosition;
			return TaskStatus.Success;
		}
	}
}