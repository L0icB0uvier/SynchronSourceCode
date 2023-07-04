using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Characters.Controls.Controllers.PlayerControllers.Hicks
{
	public class HicksAIController : PlayerAIController
	{
		protected override void Awake()
		{
			base.Awake();
			followTargetTransform = GameObject.FindWithTag("Skullface").transform;
		}
	}
}
