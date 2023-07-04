using System;
using System.Collections.Generic;
using Characters.Controls.Controllers.PlayerControllers;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using UnityEngine;

namespace UI.CallToActionUI
{
	public class ActionManager : MonoBehaviour
	{
		[SerializeField] private ActionUI _hicksFixedActionUI;
		[SerializeField] private ActionUI _skullfaceFixedActionUI;
		
		private readonly Stack<StringVariable> m_hicksActions = new Stack<StringVariable>();
		private readonly Stack<StringVariable> m_skullfaceActions = new Stack<StringVariable>();

		[Header("Listening to")]
		[SerializeField] private InteractionUIEventChannelSO _addUIActionEventChannelSo;
		[SerializeField] private InteractionUIEventChannelSO _changeUIActionEventChannelSo;
		[SerializeField] private RemoveActionUIEventChannel _removeUIActionEventChannelSo;
		[SerializeField] private ClearActionUIEventChannelSO _clearActionUIEventChannel;
		[SerializeField] private VoidEventChannelSO[] _resetActionUIEventChannels;

		[SerializeField] private ActionUISettings defaultActionUISettings;
		
		
		private void Awake()
		{
			foreach (var resetAction in _resetActionUIEventChannels)
			{
				resetAction.onEventRaised += ResetActionUI;
			}
			
			_addUIActionEventChannelSo.OnEventRaised += AddAction;
			_changeUIActionEventChannelSo.OnEventRaised += ChangeCurrentAction;
			_removeUIActionEventChannelSo.OnEventRaised += RemoveCurrentAction;
			_clearActionUIEventChannel.onEventRaised += ClearAllActions;
		}

		private void OnDestroy()
		{
			foreach (var resetAction in _resetActionUIEventChannels)
			{
				resetAction.onEventRaised -= ResetActionUI;
			}
			
			_addUIActionEventChannelSo.OnEventRaised -= AddAction;
			_changeUIActionEventChannelSo.OnEventRaised -= ChangeCurrentAction;
			_removeUIActionEventChannelSo.OnEventRaised -= RemoveCurrentAction;
			_clearActionUIEventChannel.onEventRaised -= ClearAllActions;
		}

		private void ResetActionUI()
		{
			ClearAllActions(EPlayerCharacterType.Hicks);
			ClearAllActions(EPlayerCharacterType.Skullface);
		}

		private void ClearAllActions(EPlayerCharacterType characterType)
		{
			switch (characterType)
			{
				case EPlayerCharacterType.Hicks:
					m_hicksActions.Clear();
					_hicksFixedActionUI.HideUI();
					break;
			
				case EPlayerCharacterType.Skullface:
					m_skullfaceActions.Clear();
					_skullfaceFixedActionUI.HideUI();
					break;
			}
		}

		private void AddAction(EPlayerCharacterType characterType, StringVariable actionName, ActionUISettings actionUISetting)
		{
			switch (characterType)
			{
				case EPlayerCharacterType.Hicks:
					if (m_hicksActions.Contains(actionName)) return;
					m_hicksActions.Push(actionName);
					_hicksFixedActionUI.ChangeText(m_hicksActions.Peek().Value, actionUISetting);
					break;
			
				case EPlayerCharacterType.Skullface:
					if (m_skullfaceActions.Contains(actionName)) return;
					m_skullfaceActions.Push(actionName);
					_skullfaceFixedActionUI.ChangeText(m_skullfaceActions.Peek().Value, actionUISetting);
					break;
			}
		}

		private void ChangeCurrentAction(EPlayerCharacterType characterType, StringVariable newActionName, ActionUISettings actionUISetting)
		{
			switch (characterType)
			{
				case EPlayerCharacterType.Hicks:
					m_hicksActions.Pop();
					m_hicksActions.Push(newActionName);
					_hicksFixedActionUI.ChangeText(m_hicksActions.Peek().Value, actionUISetting);
					break;
				case EPlayerCharacterType.Skullface:
					m_skullfaceActions.Pop();
					m_skullfaceActions.Push(newActionName);
					_skullfaceFixedActionUI.ChangeText(m_skullfaceActions.Peek().Value, actionUISetting);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(characterType), characterType, null);
			}
		}

		private void RemoveCurrentAction(EPlayerCharacterType characterType, StringVariable actionText)
		{
			switch (characterType)
			{
				case EPlayerCharacterType.Hicks:
					if (m_hicksActions.Count == 0 || !m_hicksActions.Contains(actionText)) return;
					
					m_hicksActions.Pop();
					
					if (m_hicksActions.Count == 0)
					{
						_hicksFixedActionUI.HideUI();
						return;
					}
				
					_hicksFixedActionUI.ChangeText(m_hicksActions.Peek().Value, defaultActionUISettings);
					break;
			
				case EPlayerCharacterType.Skullface:
					if (m_skullfaceActions.Count == 0 || !m_skullfaceActions.Contains(actionText)) return;
					m_skullfaceActions.Pop();
					if (m_skullfaceActions.Count == 0)
					{
						_skullfaceFixedActionUI.HideUI();
						return;
					}
				
					_skullfaceFixedActionUI.ChangeText(m_skullfaceActions.Peek().Value, defaultActionUISettings);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(characterType), characterType, null);
			}
		}
	}
}