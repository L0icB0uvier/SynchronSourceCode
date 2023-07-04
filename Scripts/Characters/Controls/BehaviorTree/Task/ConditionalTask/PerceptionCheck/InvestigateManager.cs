using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Characters.Controls.Controllers.AIControllers;
using Characters.Enemies.Perception;
using GeneralScriptableObjects;
using NoiseSystem;
using Pathfinding;
using UnityEngine;

namespace Characters.Controls.BehaviorTree.Task.ConditionalTask.PerceptionCheck
{
    [TaskCategory("Perception")]
    public class InvestigateManager : Conditional
    {
        public SharedAIController AIController;
	
        FieldOfView fov;

        public SharedVector2 AnomalyLocation;

        private Collider2D[] m_noiseCollider = new Collider2D[5];

        public LayerMaskVariable noiseLayerMask;

        public LayerMaskVariable obstacleLayerMask;

        private List<Noise> m_noisesHeard = new List<Noise>();
        
        public override void OnAwake()
        {
            base.OnAwake();
            fov = AIController.Value.transform.GetComponentInChildren<FieldOfView>();
        }

        public override TaskStatus OnUpdate()
        {
            Vector2 anomalyLocation = new Vector2();
            
            if (HeardSomething(out anomalyLocation) || CatchSight(out anomalyLocation))
            {
                AnomalyLocation.Value = anomalyLocation;
                return TaskStatus.Success;
            }
            
            AnomalyLocation.Value = Vector2.zero;
            return TaskStatus.Failure;
        }

        private bool HeardSomething(out Vector2 noiseLocation)
        {
            if(Physics2D.OverlapPointNonAlloc(transform.position, m_noiseCollider, noiseLayerMask.Value) == 0) 
            {
                noiseLocation = Vector2.zero;
                return false;
            }
		
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
                noiseLocation = Vector2.zero;
                return false;
            }

            if (m_noisesHeard.Count > 1)
            {
                m_noisesHeard = m_noisesHeard.OrderByDescending(x => x.EmissionTime).ToList();
            }

            Vector3 noiseGraphPosition = (Vector3)PathfindingUtilities.GetNearestNavigableNode(m_noisesHeard[0].transform.position, GraphMask.FromGraphName("MainGraph")).position;

            noiseLocation= noiseGraphPosition;
            return true;
        }

        private bool CatchSight(out Vector2 sightLocation)
        {
            if (!fov.SeeingSomething)
            {
                sightLocation = Vector2.zero;
                return false;
            }
		
            sightLocation = (Vector3)PathfindingUtilities.GetNearestNavigableNode(fov.AnomalyLocation, GraphMask.FromGraphName("MainGraph")).position ;
            return true;
        }
    }
}