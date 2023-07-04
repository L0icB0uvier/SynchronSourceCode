using GeneralScriptableObjects;
using UnityEngine;

namespace Characters.CharacterAbilities.Teleport.Resolve
{
	public class SkullfaceResolver : Resolver
	{
		[SerializeField] private FloatVariable lookingDirection;

		protected override void SetAnimatorParameters()
		{
			resolveAnimator.SetFloat(LookingDirection, lookingDirection.Value);
		}
	}
}
