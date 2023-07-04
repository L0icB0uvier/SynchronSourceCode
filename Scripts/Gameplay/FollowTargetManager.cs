using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using Pathfinding;
using UnityEngine;
using Utilities;

namespace Gameplay
{
    public class FollowTargetManager : MonoBehaviour
    {
        private Transform m_hicksTransform;
        private Transform m_skullfaceTransform;
        [SerializeField] private FloatVariable maxFollowDistance;
        [SerializeField] private BoolVariable followTargetPossible;
        [SerializeField] private BoolVariable skullfacePoweringSocket;
    
        [SerializeField] private VoidEventChannelSO[] enableChannels;
        [SerializeField] private VoidEventChannelSO[] disableChannels;
    
        private void Awake()
        {
            m_hicksTransform = GameObject.FindGameObjectWithTag("Hicks").transform;
            m_skullfaceTransform = GameObject.FindGameObjectWithTag("Skullface").transform;

            foreach (var channel in enableChannels)
            {
                channel.onEventRaised += EnableCheck;
            } 
        
            foreach (var channel in disableChannels)
            {
                channel.onEventRaised += DisableCheck;
            }
        }

        private void OnDestroy()
        {
            foreach (var channel in enableChannels)
            {
                channel.onEventRaised -= EnableCheck;
            } 
        
            foreach (var channel in disableChannels)
            {
                channel.onEventRaised -= DisableCheck;
            }
        }

        private void EnableCheck()
        {
            enabled = true;
        }

        private void DisableCheck()
        {
            enabled = false;
            followTargetPossible.Value = false;
        }

        // Update is called once per frame
        private void Update()
        {
            followTargetPossible.Value = skullfacePoweringSocket.Value == false && IsOnGround() && IsInFollowRange() && 
                                         IsTargetPositionReachable() && IsSightUnobstructed();
        }

        private bool IsOnGround()
        {
            return EnvironmentalQueryUtilities.IsOnGround(m_skullfaceTransform.position);
        }

        private bool IsInFollowRange()
        {
            Vector2 skullfacePosition = m_skullfaceTransform.position;
            Vector2 hicksPosition = m_hicksTransform.position;
        
            var dirToTarget = MathCalculation.GetDirectionalVectorBetween2Points(skullfacePosition, hicksPosition);
            var distanceToTarget = (hicksPosition - skullfacePosition).sqrMagnitude;

            var directionMaxDistance = MathCalculation.GetPointOnEllipse(skullfacePosition, maxFollowDistance.Value, dirToTarget);

            return (directionMaxDistance - skullfacePosition).sqrMagnitude > distanceToTarget;
        }

        private bool IsTargetPositionReachable()
        {
            return PathfindingUtilities.IsGraphAreaEqual(m_skullfaceTransform.position, m_hicksTransform.position, GraphMask.FromGraphName("AreaGraph"));
        }

        private bool IsSightUnobstructed()
        {
            return !EnvironmentalQueryUtilities.IsSightBlockedByCoverObstacle(m_skullfaceTransform.position, m_hicksTransform
                .position);
        }
    }
}
