using Characters.Controls.Controllers.AIControllers;
using GeneralScriptableObjects.Events;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Characters.Controls.Controllers.PlayerControllers.Hicks
{
    public class PlayerAIController : AIController
    {
        [Header("Listening to")]
        [SerializeField] private MoveCharacterToPositionChannelSO _moveCharacterToLocation;
        [SerializeField] private VoidEventChannelSO _locationReachedEvent;

        protected Transform followTargetTransform;
        
        [SerializeField][FoldoutGroup("Components")] private BehaviorDesigner.Runtime.BehaviorTree moveToLocationBT;

        public Seeker Seeker { get; private set; }

        private UnityAction m_locationReachedCallback;

        public UnityEvent onStartMoving;
        public UnityEvent onStopMoving;
        
        protected virtual void Awake()
        {
            Seeker = GetComponent<Seeker>();
        }

        protected override void InitializeBtValues()
        {
            moveToLocationBT.SetVariableValue("AIController", this);
        }

        protected virtual void OnEnable()
        {
            _moveCharacterToLocation.OnEventRaised += MoveToLocation;
            _locationReachedEvent.onEventRaised += LocationReached;
        }

        protected virtual void OnDisable()
        {
            _moveCharacterToLocation.OnEventRaised -= MoveToLocation;
            _locationReachedEvent.onEventRaised -= LocationReached;
            
            onStopMoving?.Invoke();
        }

        public void MoveToLocation(Vector2 cutsceneStartLocation, UnityAction callback)
        {
            m_locationReachedCallback = callback;
            moveToLocationBT.SetVariableValue("Location", cutsceneStartLocation);
            EnableBehaviorTree(moveToLocationBT);
            onStartMoving?.Invoke();
        } 
        
        public void MoveToLocation(Vector2 cutsceneStartLocation)
        {
            moveToLocationBT.SetVariableValue("Location", cutsceneStartLocation);
            EnableBehaviorTree(moveToLocationBT);
            onStartMoving?.Invoke();
        }

        private void LocationReached()
        {
            DisableBehaviorTree(moveToLocationBT);
            m_locationReachedCallback?.Invoke();
            m_locationReachedCallback = null;
            onStopMoving?.Invoke();
        }
    }
}
