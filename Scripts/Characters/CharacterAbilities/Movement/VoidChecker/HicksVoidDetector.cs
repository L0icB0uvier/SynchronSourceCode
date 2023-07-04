using System;
using GeneralEnums;
using GeneralScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Characters.CharacterAbilities.Movement.VoidChecker
{
	public class HicksVoidDetector : MonoBehaviour
	{
		[SerializeField] private float collisionCheckDistance = .5f;

		[SerializeField] private BoolVariable teleportationInProgress;
		
		public UnityEvent onCharacterAboveVoid;

		[SerializeField] private CharacterDirectionLocker directionLocker;
		
		private void FixedUpdate()
		{
			UpdateVoidDetector();

			CheckIfCharacterIsAboveVoid();
		}

		private void CheckIfCharacterIsAboveVoid()
		{
			if (!teleportationInProgress.Value && !EnvironmentalQueryUtilities.IsOnGround(transform.position, .5f))
			{
				onCharacterAboveVoid?.Invoke();
			}
		}

		public void UpdateVoidDetector()
		{
			foreach (EIsometricCardinal4DiagonalDirection cardinalDirection in Enum.GetValues(typeof(EIsometricCardinal4DiagonalDirection)))
			{
				var point = MathCalculation.GetPointOnEllipse(transform.position, collisionCheckDistance, (int)
					cardinalDirection);
				directionLocker.UpdateDirectionLock(cardinalDirection, !EnvironmentalQueryUtilities.IsOnGround(point));
			}
		}
	}
}

