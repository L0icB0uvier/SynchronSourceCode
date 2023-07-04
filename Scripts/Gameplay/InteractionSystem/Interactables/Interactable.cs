using System;
using System.Linq;
using Characters.Controls.Controllers.PlayerControllers;
using Gameplay.InteractionSystem.Interacters;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using Sirenix.OdinInspector;
using UI.CallToActionUI;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.InteractionSystem.Interactables
{
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] private InteracterProfileSO[] authorizedInteracters;
        public InteracterProfileSO[] AuthorizedInteracters => authorizedInteracters;
        
        [SerializeField][FoldoutGroup("ActionUI")] private ActionUISettings actionUISetting;
        
        [Header("Broadcasting to")]
        [SerializeField][FoldoutGroup("ActionUI/Broadcast channels")] protected InteractionUIEventChannelSO _addUIActionEventChannelSo;
        [SerializeField][FoldoutGroup("ActionUI/Broadcast channels")] protected InteractionUIEventChannelSO _changeUIActionEventChannelSo;
        [SerializeField][FoldoutGroup("ActionUI/Broadcast channels")] protected RemoveActionUIEventChannel _removeUIActionEventChannelSo;
        [SerializeField][FoldoutGroup("ActionUI/Broadcast channels")] private ClearActionUIEventChannelSO _clearActionUIEventChannel;
        
        protected Interacter currentInteracter;
        
        
        [FoldoutGroup("Events")] public UnityEvent onInteracterEnter;
        [FoldoutGroup("Events")] public UnityEvent onInteracterExit;
        
        [FoldoutGroup("Events")] public UnityEvent OnInteractionAllowed;
        [FoldoutGroup("Events")] public UnityEvent OnInteractionForbidden;
        
        private bool IsInteracterAuthorized(Interacter interacter)
        {
            return AuthorizedInteracters.Contains(interacter.InteracterProfile);
        }
        
        protected abstract bool IsInteractionPossible();

        public abstract bool TryInteraction(Interacter interacter);
        
        public virtual void InteracterEntered(Interacter interacter)
        {
            currentInteracter = interacter;
            onInteracterEnter?.Invoke();
            interacter.OnInteractableEntered(this);
            
            if (IsInteractionPossible())
            {
                DisplayActionText(interacter);
            }
        }

        public virtual void InteracterExited(Interacter interacter)
        {
            if (interacter != currentInteracter) return;
            currentInteracter = null;
            onInteracterExit?.Invoke();
            interacter.OnInteractableExited(this);
            
            HideActionText(interacter);
        }

        /*private void OnTriggerEnter2D(Collider2D other)
        {
            if (currentInteracter != null) return;
            
            var interacter = other.GetComponent<Interacter>();

            if (interacter != null && IsInteracterAuthorized(interacter))
            {
                currentInteracter = interacter;
                if(IsInteractionPossible()) InteracterEntered(interacter);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var interacter = other.GetComponent<Interacter>();

            if (currentInteracter == null || interacter != currentInteracter) return;
            currentInteracter = null;
            if(interactionAllowed) InteracterExited(interacter);
        }*/

        protected void DisplayActionText(Interacter interacter)
        {
            switch (interacter.InteracterProfile.interacterActionType)
            {
                case InteracterProfileSO.EInteracterActionType.ShowNoActionText:
                    break;
                case InteracterProfileSO.EInteracterActionType.ShowHicksActionText:
                    _addUIActionEventChannelSo.RaiseEvent(EPlayerCharacterType.Hicks, GetActionText(), actionUISetting);
                    break;
                case InteracterProfileSO.EInteracterActionType.ShowSkullfaceActionText:
                    _addUIActionEventChannelSo.RaiseEvent(EPlayerCharacterType.Skullface, GetActionText(), actionUISetting);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void HideActionText(Interacter interacter)
        {
            switch (interacter.InteracterProfile.interacterActionType)
            {
                case InteracterProfileSO.EInteracterActionType.ShowNoActionText:
                    break;
                case InteracterProfileSO.EInteracterActionType.ShowHicksActionText:
                    _removeUIActionEventChannelSo.RaiseEvent(EPlayerCharacterType.Hicks, GetActionText());
                    break;
                case InteracterProfileSO.EInteracterActionType.ShowSkullfaceActionText:
                    _removeUIActionEventChannelSo.RaiseEvent(EPlayerCharacterType.Skullface, GetActionText());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void ChangeActionText()
        {
            if (currentInteracter == null) return;
            
            switch (currentInteracter.InteracterProfile.interacterActionType)
            {
                case InteracterProfileSO.EInteracterActionType.ShowNoActionText:
                    break;
                case InteracterProfileSO.EInteracterActionType.ShowHicksActionText:
                    _changeUIActionEventChannelSo.RaiseEvent(EPlayerCharacterType.Hicks, GetActionText(), actionUISetting);
                    break;
                case InteracterProfileSO.EInteracterActionType.ShowSkullfaceActionText:
                    _changeUIActionEventChannelSo.RaiseEvent(EPlayerCharacterType.Skullface, GetActionText(), actionUISetting);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void ClearActionUI()
        {
            if (currentInteracter == null) return;
            
            switch (currentInteracter.InteracterProfile.interacterActionType)
            {
                case InteracterProfileSO.EInteracterActionType.ShowNoActionText:
                    break;
                case InteracterProfileSO.EInteracterActionType.ShowHicksActionText:
                    _clearActionUIEventChannel.RaiseEvent(EPlayerCharacterType.Hicks);
                    break;
                case InteracterProfileSO.EInteracterActionType.ShowSkullfaceActionText:
                    _clearActionUIEventChannel.RaiseEvent(EPlayerCharacterType.Skullface);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void AllowInteraction()
        {
            OnInteractionAllowed?.Invoke();

            if (currentInteracter != null && IsInteractionPossible())
            {
                InteracterEntered(currentInteracter);
            }
        }

        public void ForbidInteraction()
        {
            OnInteractionForbidden?.Invoke();
            
            if(currentInteracter != null) InteracterExited(currentInteracter);
            
        }

        protected abstract StringVariable GetActionText();
    }
}