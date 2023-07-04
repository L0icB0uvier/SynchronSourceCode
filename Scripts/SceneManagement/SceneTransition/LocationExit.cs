using System;
using System.Collections;
using Characters.Controls.Controllers.PlayerControllers;
using GeneralEnums;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using GeneralScriptableObjects.SceneData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace SceneManagement.SceneTransition
{
	[RequireComponent(typeof(PolygonCollider2D))]
	public class LocationExit : MonoBehaviour
	{
		[FoldoutGroup("Transition settings")][SerializeField] private EIsometricCardinal4DiagonalDirection _destinationDirection;
		[FoldoutGroup("Transition settings")][SerializeField] private FloatReference _transitionMoveDistance;
		[FoldoutGroup("Transition settings")][SerializeField] private FloatReference _delayBeforeSceneChange;
		[FoldoutGroup("Transition settings")][SerializeField] private GameSceneSO _locationToLoad = default;
		[FoldoutGroup("Transition settings")][SerializeField] private PathSO _leadsToPath = default;
		
		[FoldoutGroup("Broadcasting on")][SerializeField] private LoadEventChannelSO _locationExitLoadChannel;
		[FoldoutGroup("Broadcasting on")][SerializeField] private LocationChangeChannelSO _locationChangeChannel;
		[FoldoutGroup("Broadcasting on")][SerializeField] private MoveCharacterToPositionChannelSO _moveHicksEvent;
		[FoldoutGroup("Broadcasting on")][SerializeField] private MoveCharacterToPositionChannelSO _moveSkullfaceEvent;
		[FoldoutGroup("Broadcasting on")][SerializeField] private VoidEventChannelSO _sceneTransitionStartEvent;
		[FoldoutGroup("Broadcasting on")][SerializeField] private VoidEventChannelSO _requestSaveChannel;
		
		[FoldoutGroup("Listening to")][SerializeField] private VoidEventChannelSO _respawnStartChannel;
		[FoldoutGroup("Listening to")][SerializeField] private VoidEventChannelSO _transitionComplete;
		
		[FoldoutGroup("Storage SO")][SerializeField] private PathStorageSO _pathStorage = default; //This is where the last path taken will be stored
		
		private Transform m_hicksTransform;
		private Transform m_skullfaceTransform;

		private Collider2D m_exitCollider;

		[SerializeField] private CharacterDirectionLocker hicksDirectionLocker;
		[SerializeField] private CharacterDirectionLocker skullfaceDirectionLocker;

		[SerializeField] private UnityEvent onTransitionStart;

		private void Awake()
		{
			m_hicksTransform = GameObject.FindGameObjectWithTag("Hicks").transform;
			m_skullfaceTransform = GameObject.FindGameObjectWithTag("Skullface").transform;

			m_exitCollider = GetComponent<Collider2D>();
			
			_transitionComplete.onEventRaised += EnableTransition;
			_respawnStartChannel.onEventRaised += DisableTransition;
			
			DisableTransition();
		}
		
		private void OnDestroy()
		{
			_transitionComplete.onEventRaised -= EnableTransition;
			_respawnStartChannel.onEventRaised -= DisableTransition;
			
			StopAllCoroutines();
		}

		public void LockCharacterDirection(EPlayerCharacterType characterType)
		{
			switch (characterType)
			{
				case EPlayerCharacterType.Hicks:
					hicksDirectionLocker.LockUniqueDirection(_destinationDirection);
					break;
				case EPlayerCharacterType.Skullface:
					skullfaceDirectionLocker.LockUniqueDirection(_destinationDirection);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(characterType), characterType, null);
			}
		}

		public void UnLockCharacterDirection(EPlayerCharacterType characterType)
		{
			switch (characterType)
			{
				case EPlayerCharacterType.Hicks:
					hicksDirectionLocker.Reset();
					break;
				case EPlayerCharacterType.Skullface:
					skullfaceDirectionLocker.Reset();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(characterType), characterType, null);
			}
		}
		
		public void ChangeScene()
		{
			_sceneTransitionStartEvent.RaiseEvent();
			_requestSaveChannel.RaiseEvent();
			onTransitionStart?.Invoke();
			
			var moveDirection = MathCalculation.ConvertAngleToDirection((int)_destinationDirection);
			var hicksMoveToLocation = (Vector2)m_hicksTransform.position + moveDirection *_transitionMoveDistance.Value;
			var skullfaceMoveToLocation = (Vector2)m_skullfaceTransform.position + moveDirection * _transitionMoveDistance.Value;

			_moveHicksEvent.RaiseEvent(hicksMoveToLocation, null);
			_moveSkullfaceEvent.RaiseEvent(skullfaceMoveToLocation, null);
			
			_pathStorage.lastPathTaken = _leadsToPath;
			
			if (_locationToLoad.sceneType == GameSceneSO.GameSceneType.Location)
			{
				_locationChangeChannel.RaiseEvent(_locationToLoad as LocationSO);
			}
			
			DisableTransition();
			StartCoroutine(ChangeSceneInternal());
		}

		private IEnumerator ChangeSceneInternal()
		{
			yield return new WaitForSeconds(_delayBeforeSceneChange.Value);
			_locationExitLoadChannel.RaiseEvent(_locationToLoad, false, true);
		}

		private void EnableTransition()
		{
			m_exitCollider.enabled = true;
		}

		private void DisableTransition()
		{
			m_exitCollider.enabled = false;
		}
	}
}