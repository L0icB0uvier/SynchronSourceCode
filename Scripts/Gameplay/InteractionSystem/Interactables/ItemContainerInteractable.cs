using System.Collections;
using Characters.CharacterAbilities.Inventory;
using Gameplay.Collectibles;
using Gameplay.InteractionSystem.Interacters;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.InteractionSystem.Interactables
{
    public class ItemContainerInteractable : Interactable
    {
        [SerializeField] private ItemContainer _itemContainer;
        
        [SerializeField][FoldoutGroup("ActionUI/Action text")] private StringVariable containerContainItemText;
        [SerializeField][FoldoutGroup("ActionUI/Action text")] private StringVariable containerEmptyText;

        [SerializeField] private VoidEventChannelSO _objectInteractedChannel;
        [SerializeField] private BoolEventChannelSO collectibleUsableChannel;

        [FoldoutGroup("Events")] public UnityEvent onItemPickedUp;
        [FoldoutGroup("Events")] public UnityEvent onItemStored;

        private bool m_interacted;
        [SerializeField] private FloatVariable interactionDelay;
        
        
        public override void InteracterEntered(Interacter interacter)
        {
            base.InteracterEntered(interacter);
            collectibleUsableChannel.RaiseEvent(true);
        }

        public override void InteracterExited(Interacter interacter)
        {
            base.InteracterExited(interacter);
            collectibleUsableChannel.RaiseEvent(false);
        }

        public override bool TryInteraction(Interacter interacter)
        {
            if (!IsInteractionPossible()) return false;
            
            var inventory = interacter.transform.root.GetComponent<Inventory>();

            if (_itemContainer.ContainItem)
            {
                HideActionText(interacter);
                _itemContainer.TakeItem();
                inventory.AddItem(_itemContainer.ContainerObjectType);
                collectibleUsableChannel.RaiseEvent(true);
                onItemPickedUp?.Invoke();
                StartCoroutine(WaitForInteractionDelay());
            }

            else
            {
                HideActionText(interacter);
                _itemContainer.StoreItem();
                inventory.RemoveItem(_itemContainer.ContainerObjectType);
                onItemStored?.Invoke();
                StartCoroutine(WaitForInteractionDelay());
            }
            
            _objectInteractedChannel.RaiseEvent();
            return true;
        }

        private IEnumerator WaitForInteractionDelay()
        {
            m_interacted = true;
            yield return new WaitForSeconds(interactionDelay.Value);
            m_interacted = false;
            
            if (IsInteractionPossible())
            {
                DisplayActionText(currentInteracter);
            }
        }
        
        protected override bool IsInteractionPossible()
        {
            if (m_interacted || !currentInteracter) return false;
            
            var inventory = currentInteracter.transform.root.GetComponent<Inventory>();

            if (!inventory) return false;

            return _itemContainer.ContainItem switch
            {
                true => !inventory.ContainItem(_itemContainer.ContainerObjectType),
                false => inventory.ContainItem(_itemContainer.ContainerObjectType)
            };
        }

        protected override StringVariable GetActionText()
        {
            return _itemContainer.ContainItem ? containerContainItemText : containerEmptyText;
        }
    }
}
