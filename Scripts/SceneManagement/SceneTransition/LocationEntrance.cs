using System;
using GeneralEnums;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace SceneManagement
{
	public class LocationEntrance : MonoBehaviour
	{
		[FoldoutGroup("Transition settings")][SerializeField] private bool _moveCharacterWhenEnteringLocation;
		[FoldoutGroup("Transition settings")][SerializeField] private EIsometricCardinal4DiagonalDirection _destinationFacingDirection;
		[FoldoutGroup("Transition settings")][SerializeField][ShowIf("_moveCharacterWhenEnteringLocation")] private FloatReference _transitionMoveDistance;
		[FoldoutGroup("Transition settings")][SerializeField] private PathSO _entrancePath;
		public PathSO EntrancePath => _entrancePath;

		[FoldoutGroup("Listening to")][SerializeField] private VoidEventChannelSO _playerCharactersSpawnedChannel;

		[FoldoutGroup("Broadcasting to")][SerializeField][ShowIf("_moveCharacterWhenEnteringLocation")] private MoveCharacterToPositionChannelSO _moveHicksEvent = default;
		[FoldoutGroup("Broadcasting to")][SerializeField][ShowIf("_moveCharacterWhenEnteringLocation")] private MoveCharacterToPositionChannelSO _moveSkullfaceEvent = default;
		[FoldoutGroup("Broadcasting to")][SerializeField] private VoidEventChannelSO _sceneTransitionStartEvent;
		[FoldoutGroup("Broadcasting to")][SerializeField] private VoidEventChannelSO _sceneTransitionEndEvent;
		
		[FoldoutGroup("SO ref")][SerializeField] private FloatVariable _hicksLookingDirection;
		[FoldoutGroup("SO ref")][SerializeField] private FloatVariable _skullfaceLookingDirection;
		[FoldoutGroup("SO ref")][SerializeField] private PathStorageSO _pathStorage = default; //This is where the last path taken has been stored

		[SerializeField] private UnityEvent onSceneTransitionStart;
		[SerializeField] private UnityEvent onSceneTransitionEnd;
		
		private int m_characterReachedLocationCount;
		
		public Transform HicksEntranceLocation { get; private set; }
		public Transform SkullfaceEntranceLocation { get; private set; }

		private void Awake()
		{
			GetEntranceLocations();
		}

		private void GetEntranceLocations()
		{
			HicksEntranceLocation = transform.GetChild(0);
			SkullfaceEntranceLocation = transform.GetChild(1);
		}

		private void Start()
		{
			if (_pathStorage.lastPathTaken != _entrancePath) return;
			
			_playerCharactersSpawnedChannel.onEventRaised += StartTransition;
		}
		
		private void SetPlayerCharactersLookingDirection(int destinationDirection)
		{
			_hicksLookingDirection.SetValue(destinationDirection);
			_skullfaceLookingDirection.SetValue(destinationDirection);
		}

		private void StartTransition()
		{
			onSceneTransitionStart?.Invoke();
			m_characterReachedLocationCount = 0;
			_sceneTransitionStartEvent.RaiseEvent();
			SetPlayerCharactersLookingDirection((int)_destinationFacingDirection);
			if (_moveCharacterWhenEnteringLocation)
			{
				var moveDirection = MathCalculation.ConvertAngleToDirection((int)_destinationFacingDirection);
				var hicksEnterMoveToLocation = (Vector2)HicksEntranceLocation.position + moveDirection 
					*_transitionMoveDistance;
				var skullfaceEnterMoveToLocation = (Vector2)SkullfaceEntranceLocation.position + moveDirection * 
					_transitionMoveDistance;

				//Send event to player AI controllers
				_moveHicksEvent.RaiseEvent(hicksEnterMoveToLocation, CharacterReachedLocation);
				_moveSkullfaceEvent.RaiseEvent(skullfaceEnterMoveToLocation, CharacterReachedLocation);
			}

			else
			{
				_sceneTransitionEndEvent.RaiseEvent();
				_playerCharactersSpawnedChannel.onEventRaised -= StartTransition;
			}
		}

		private void CharacterReachedLocation()
		{
			m_characterReachedLocationCount++;
			if (m_characterReachedLocationCount == 2)
			{
				TransitionComplete();	
			}
		}

		private void TransitionComplete()
		{
			_sceneTransitionEndEvent.RaiseEvent();
			_playerCharactersSpawnedChannel.onEventRaised -= StartTransition;
			onSceneTransitionEnd?.Invoke();
		}
	}
}