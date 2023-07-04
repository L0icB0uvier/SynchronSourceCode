using System.Collections;
using Characters.CharacterAbilities.Inventory;
using Gameplay.Collectibles;
using GeneralScriptableObjects.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.InteractionSystem.InteractionManagers
{
    public class ItemInteractionManager : MonoBehaviour
    {
        [SerializeField] private InventorySlots _hicksInventory;
        [SerializeField] private ItemContainer m_itemContainer;
        [SerializeField] private PickableItem itemTracked;

        public UnityEvent onObjectInteractionBecomePossible;
        public UnityEvent onObjectInteractionBecomeImpossible;

        private void OnEnable()
        {
            m_itemContainer.onContainerChanged += ResolveSwitchState;
            _hicksInventory.onInventoryChanged += ResolveSwitchState;
        }

        private void OnDisable()
        {
            m_itemContainer.onContainerChanged -= ResolveSwitchState;
            _hicksInventory.onInventoryChanged -= ResolveSwitchState;
        }

        private void ResolveSwitchState()
        {
            StopAllCoroutines();
            StartCoroutine(ResolveSwitch());
        }

        private IEnumerator ResolveSwitch()
        {
            yield return new WaitForSeconds(.1f);
        
            if (m_itemContainer.ContainItem && !_hicksInventory.ContainItem(itemTracked))
            {
                onObjectInteractionBecomePossible?.Invoke();
                yield return null;
            }

            if (m_itemContainer.ContainItem && _hicksInventory.ContainItem(itemTracked))
            {
                onObjectInteractionBecomeImpossible?.Invoke();
                yield return null;
            }

            if (!m_itemContainer.ContainItem && _hicksInventory.ContainItem(itemTracked))
            {
                onObjectInteractionBecomePossible?.Invoke();
                yield return null;
            }

            if (!m_itemContainer.ContainItem && !_hicksInventory.ContainItem(itemTracked))
            {
                onObjectInteractionBecomeImpossible?.Invoke();
                yield return null;
            }
        }
        
        public void DisableInteraction()
        {
            onObjectInteractionBecomeImpossible?.Invoke();
            enabled = false;
        }

        public void EnableInteraction()
        {
            enabled = true;
            ResolveSwitchState();
        }
    }
    
   
}
