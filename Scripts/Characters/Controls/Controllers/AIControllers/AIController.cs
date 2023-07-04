using System;
using System.Collections;
using BehaviorDesigner.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Controls.Controllers.AIControllers
{
	public abstract class AIController : Controller
	{
		protected virtual void Start()
		{
			InitializeBtValues();
		}

		//Initialise behavior tree values
		protected abstract void InitializeBtValues();

		public void ResetBehaviorTree(BehaviorDesigner.Runtime.BehaviorTree behaviorTree)
		{
			StartCoroutine(RestartBehaviorTree(behaviorTree));
		}
		
		public void EnableBehaviorTree(BehaviorDesigner.Runtime.BehaviorTree behaviorTree)
		{
			if(behaviorTree.enabled) return;
			behaviorTree.enabled = true;
			behaviorTree.EnableBehavior();
		}
		
		public void DisableBehaviorTree(BehaviorDesigner.Runtime.BehaviorTree behaviorTree)
		{
			if (!behaviorTree.enabled) return;
			behaviorTree.enabled = false;
			behaviorTree.DisableBehavior();
		}

		private IEnumerator RestartBehaviorTree(BehaviorDesigner.Runtime.BehaviorTree behaviorTree)
		{
			DisableBehaviorTree(behaviorTree);
			yield return new WaitForSecondsRealtime(.2f);
			EnableBehaviorTree(behaviorTree);
		}
	}

	[Serializable]
	public class SharedAIController : SharedVariable<AIController>
	{
		public static implicit operator SharedAIController(AIController value)
		{ return new SharedAIController { Value = value }; }
	}
}