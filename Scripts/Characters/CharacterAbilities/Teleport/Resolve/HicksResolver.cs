using GeneralScriptableObjects;
using UnityEngine;

namespace Characters.CharacterAbilities.Teleport.Resolve
{
	public class HicksResolver : Resolver
	{
		private static readonly int stealth = Animator.StringToHash("Stealth");
		[SerializeField] private FloatVariable lookingDirection;
		[SerializeField] private BoolVariable stealthModeActive;
		
		protected override void SetAnimatorParameters()
		{
			resolveAnimator.SetFloat(LookingDirection, lookingDirection.Value);
			resolveAnimator.SetBool(stealth,  stealthModeActive.Value);
		}
	}
}
