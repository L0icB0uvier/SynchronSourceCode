using Characters.Controls.Controllers.PlayerControllers.Hicks;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Characters.Controls.Controllers.AIControllers.PlayerDrone
{
	public class SkullfaceAIController : PlayerAIController
	{
		[SerializeField] private VoidEventChannelSO startFollowInputChannel;
		[SerializeField] private VoidEventChannelSO stopFollowInputChannel;

		[SerializeField] private BoolVariable followTargetPossible;

		[SerializeField][FoldoutGroup("Components")] private BehaviorDesigner.Runtime.BehaviorTree followBT;

		[SerializeField] private UnityEvent onStartFollowingTarget;
		[SerializeField] private UnityEvent onStopFollowingTarget;
		
		
		protected override void Awake()
		{
			base.Awake();
			followTargetTransform = GameObject.FindWithTag("Hicks").transform;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			startFollowInputChannel.onEventRaised += StartFollowingTarget;
			stopFollowInputChannel.onEventRaised += StopFollowingTarget;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			startFollowInputChannel.onEventRaised -= StartFollowingTarget;
			stopFollowInputChannel.onEventRaised -= StopFollowingTarget;
		}

		protected override void InitializeBtValues()
		{
			base.InitializeBtValues();
			followBT.SetVariableValue("AIController", this);
		}
		
		public void StartFollowingTarget()
		{
			if (!followTargetPossible.Value) return;
            
			onStartFollowingTarget?.Invoke();
			followBT.SetVariableValue("FollowTarget", followTargetTransform);
			EnableBehaviorTree(followBT);
		}

		public void StopFollowingTarget()
		{
			onStopFollowingTarget?.Invoke();
			followBT.SetVariableValue("FollowTarget", null);
			DisableBehaviorTree(followBT);
		}
	}
}