using System.Linq;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Characters.Controls.Controllers.PlayerControllers
{
	public abstract class PlayerController : Controller
	{
		[Header("Listening to")]
		[SerializeField] private VoidEventChannelSO[] loseCharacterMovementControlChannels;
		[SerializeField] private VoidEventChannelSO[] gainCharacterMovementControlChannels;

		[FormerlySerializedAs("onSceneTransitionStart")][FoldoutGroup("Events")] public UnityEvent onLoseCharacterControl;
		[FormerlySerializedAs("onSceneTransitionEnd")][FoldoutGroup("Events")] public UnityEvent onGainCharacterControl;

		[SerializeField] private BoolVariableNotifyChange[] disableMovementOnTrue;

		protected virtual void Awake()
		{
			foreach (var channel in loseCharacterMovementControlChannels)
			{
				channel.onEventRaised += LoseCharacterControl;
			}
			
			foreach (var channel in gainCharacterMovementControlChannels)
			{
				channel.onEventRaised += GainCharacterControl;
			}

			foreach (var boolVar in disableMovementOnTrue)
			{
				boolVar.onValueChanged += ShouldDisableMovement;
			}
		}
		
		protected void OnDestroy()
		{
			foreach (var channel in loseCharacterMovementControlChannels)
			{
				channel.onEventRaised -= LoseCharacterControl;
			}
			
			foreach (var channel in gainCharacterMovementControlChannels)
			{
				channel.onEventRaised -= GainCharacterControl;
			}
			
			foreach (var boolVar in disableMovementOnTrue)
			{
				boolVar.onValueChanged -= ShouldDisableMovement;
				boolVar.SetValue(false);
			}
		}

		private void ShouldDisableMovement()
		{
			if (disableMovementOnTrue.Any(x => x.Value == true))
			{
				LoseCharacterControl();
			}

			else
			{
				GainCharacterControl();
			}
		}
		
		public void LoseCharacterControl()
		{
			onLoseCharacterControl?.Invoke();
		}

		public void GainCharacterControl()
		{
			onGainCharacterControl?.Invoke();
		}
	}

	public enum EPlayerCharacterType{Hicks, Skullface}
}