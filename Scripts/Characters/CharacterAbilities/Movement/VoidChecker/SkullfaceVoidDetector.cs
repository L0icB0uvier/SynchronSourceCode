using System;
using GeneralEnums;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Characters.CharacterAbilities.Movement.VoidChecker
{
	public class SkullfaceVoidDetector : MonoBehaviour
	{
		private Rigidbody2D m_rb2d;
		private bool m_previouslyAboveVoid;

		[SerializeField] private LayerMask groundLayerMask;
		
		
		[FoldoutGroup("Data Container")][SerializeField] private BoolVariable aboveVoid;
		[FoldoutGroup("Data Container")][SerializeField] private BoolVariable platformNearby;
		[FoldoutGroup("Data Container")][SerializeField] private Vector2Variable lastGroundPos;
		[FoldoutGroup("Data Container")][SerializeField] private BoolVariable teleportInProgress;
		
		[FoldoutGroup("Settings")][SerializeField] private FloatReference platformCheckDistance;
		[FoldoutGroup("Settings")][SerializeField] private FloatReference fallSpeedThreshold;
		[FoldoutGroup("Settings")][SerializeField] private FloatReference preventFallSpeedThreshold;
		[SerializeField] private FloatReference preventFallSlowMotionTimeScale;
		[SerializeField] private BoolVariable isTeleportationPossible;
		
		[FoldoutGroup("Events")] public UnityEvent onExitPlatform;
		[FoldoutGroup("Events")] public UnityEvent onEnterPlatform;
		[FoldoutGroup("Events")] public UnityEvent onStartFalling;
		[FoldoutGroup("Events")] public UnityEvent onPreventFallingModeStart;
		[FoldoutGroup("Events")] public UnityEvent onPreventFallingModeEnd;

		[SerializeField] private ChangeTimeScaleEventChannel changeTimeScaleEventChannel;

		private bool m_preventFallModeActive;

		[SerializeField] private bool showPlatformNearestPositionGizmos;
		
		
		private void Awake()
		{
			m_rb2d = GetComponent<Rigidbody2D>();
		}

		private void OnDisable()
		{
			platformNearby.SetValue(false);
			aboveVoid.SetValue(false);
		}

		private void FixedUpdate()
		{
			m_previouslyAboveVoid = aboveVoid.Value;
			aboveVoid.SetValue(!EnvironmentalQueryUtilities.IsOnGround(transform.position, .5f));

			if (aboveVoid.Value)
			{
				if(!m_previouslyAboveVoid) onExitPlatform?.Invoke();

				platformNearby.SetValue(IsPlatformNearby());

				if (platformNearby.Value)
				{
					lastGroundPos.SetValue(SetLastGroundPosition());
					StopPreventFall();
				}

				else
				{
					if (isTeleportationPossible.Value && !teleportInProgress.Value && m_rb2d.velocity.sqrMagnitude < 
					preventFallSpeedThreshold * preventFallSpeedThreshold)
					{
						StartPreventFall();
					}

					if (!teleportInProgress.Value && m_rb2d.velocity.sqrMagnitude < fallSpeedThreshold * fallSpeedThreshold)
					{
						onStartFalling?.Invoke();
						StopPreventFall();
					}
				}
			}

			else if (!aboveVoid.Value && m_previouslyAboveVoid)
			{
				onEnterPlatform?.Invoke();
				StopPreventFall();
			}
		}

		private void StartPreventFall()
		{
			if (m_preventFallModeActive) return;
			
			onPreventFallingModeStart?.Invoke();
			m_preventFallModeActive = true;
			changeTimeScaleEventChannel.RaiseEvent(preventFallSlowMotionTimeScale, true, .2f);
		}
		
		public void StopPreventFall()
		{
			if (!m_preventFallModeActive) return;
			
			onPreventFallingModeEnd?.Invoke();
			m_preventFallModeActive = false;
			changeTimeScaleEventChannel.RaiseEvent(1, true, .2f);
		}
	
		private bool IsPlatformNearby()
		{
			foreach (EIsometricCardinal8Direction direction in Enum.GetValues(typeof(EIsometricCardinal8Direction)))
			{
				if (FoundGround((int)direction) && !FoundObstacle((int)direction)) return true;
			}

			return false;
		}

		private bool FoundGround(int direction, out Vector2 groundPos)
		{
			var hit = Physics2D.Linecast(transform.position, (Vector2)transform.position + MathCalculation
				.ConvertAngleToDirection(direction) * platformCheckDistance.Value, groundLayerMask.value);
			if (!hit)
			{
				groundPos = Vector2.zero;
				return false;
			}

			groundPos = hit.point;
			return true;
		}
		private bool FoundGround(int direction)
		{
			var hit = Physics2D.Linecast(transform.position, (Vector2)transform.position + MathCalculation
				.ConvertAngleToDirection(direction) * platformCheckDistance.Value, groundLayerMask.value);
			if (!hit)
			{
				return false;
			}
			
			return true;
		}
		
		private bool FoundObstacle(int direction)
		{
			var position = transform.position;
			return EnvironmentalQueryUtilities.IsSightBlockedByCoverObstacle(position, (Vector2)position + MathCalculation
				.ConvertAngleToDirection(direction) * platformCheckDistance.Value);
		}

		private Vector2 SetLastGroundPosition()
		{
			Vector2 nearestGroundPos = Vector2.zero;
			float sqrDistToNearestGroundPos = 1000;
			
			foreach (EIsometricCardinal8Direction direction in Enum.GetValues(typeof(EIsometricCardinal8Direction)))
			{
				Vector2 groundPos;
				if(FoundGround((int)direction, out groundPos) && !FoundObstacle((int)direction))
				{
					if (nearestGroundPos == Vector2.zero)
					{
						nearestGroundPos = groundPos;
						sqrDistToNearestGroundPos = (nearestGroundPos - (Vector2)transform.position).sqrMagnitude;
					}

					else
					{
						var sqrDistToGroundPos = (groundPos - (Vector2)transform.position).sqrMagnitude;

						if (!(sqrDistToGroundPos < sqrDistToNearestGroundPos)) continue;
						
						nearestGroundPos = groundPos;
						sqrDistToNearestGroundPos = sqrDistToGroundPos;
					}
				}
			}
			
			return nearestGroundPos;
		}

		public void ResetVoidDetector()
		{
			aboveVoid.SetValue(false);
			platformNearby.SetValue(true);
		}

#if  UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (!showPlatformNearestPositionGizmos) return;
			
			Handles.DrawSolidDisc(lastGroundPos.Value, Vector3.forward, .5f);
		}
#endif
	}
	
}