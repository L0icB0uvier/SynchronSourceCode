using System.Collections;
using Characters.CharacterAbilities.Inventory;
using Gameplay.Collectibles;
using GeneralScriptableObjects.Events;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InventoryUI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private InventorySlots inventorySlots;
        [SerializeField] private CollectibleUIEffect collectibleUIEffect;
        
        [SerializeField] private CollectibleUIIcon powerCellUI;
        [SerializeField] private CollectibleUIIcon transmitterUI;

        [SerializeField] private PickableItem powerCell;
        [SerializeField] private PickableItem transmitter;
        
        [SerializeField] private ItemContainerInteractedEventChannelSO itemPickedUpEventChannel;
        [SerializeField] private ItemContainerInteractedEventChannelSO itemStoredEventChannel;

        [SerializeField] private VoidEventChannelSO initializeChannel;
        
        private float m_currentFadeDuration;
        [SerializeField] private float fadeDuration;

        [SerializeField]
        private Image bar;
        
        [SerializeField]
        private float fadedOutAlpha;
    
        [SerializeField]
        private float fadedInAlpha;

        private bool m_barVisible;
        
        private void Start()
        {
            StartCoroutine(InitializeWithDelay());
        }

        private IEnumerator InitializeWithDelay()
        {
            yield return null;
            Initialize();
        }
        
        private void OnEnable()
        {
            initializeChannel.onEventRaised += Initialize;
            itemPickedUpEventChannel.OnEventRaised += ItemAddedToInventory;
            itemStoredEventChannel.OnEventRaised += ItemRemovedFromInventory;
        }

        private void OnDisable()
        {
            initializeChannel.onEventRaised -= Initialize;
            itemPickedUpEventChannel.OnEventRaised -= ItemAddedToInventory;
            itemStoredEventChannel.OnEventRaised -= ItemRemovedFromInventory;
        }

        private void ItemAddedToInventory(ItemContainer item)
        {
            var screenPos = GetItemIconScreenPosition(item.ContainerObjectType);
            var containerScreenPos = UnityEngine.Camera.main.WorldToScreenPoint(item.transform.position);
            collectibleUIEffect.PlayCollectibleAcquiredAnimation(item,containerScreenPos, screenPos, 
            OnAcquireCollectibleAnimationPlayed);
        }

        private void OnAcquireCollectibleAnimationPlayed(PickableItem item)
        {
            ShowItemIcon(item, true);
        }

        private void ItemRemovedFromInventory(ItemContainer item)
        {
            HideItemIcon(item.ContainerObjectType);
            var screenPos = GetItemIconScreenPosition(item.ContainerObjectType);
            var containerScreenPos = UnityEngine.Camera.main.WorldToScreenPoint(item.transform.position);
            collectibleUIEffect.PlayCollectibleUsed(item, screenPos, containerScreenPos, OnCollectibleStoredAnimationComplete);
        }

        private void OnCollectibleStoredAnimationComplete(ItemContainer item)
        {
            item.ItemStored();
        }

        private void Initialize()
        {
            switch (inventorySlots.ContainItem(powerCell))
            {
                case true:
                    ShowItemIcon(powerCell, false);
                    break;
                case false:
                    HideItemIcon(powerCell);
                    break;
            }
           
            switch (inventorySlots.ContainItem(transmitter))
            {
                case true:
                    ShowItemIcon(transmitter, false);
                    break;
                case false:
                    HideItemIcon(transmitter);
                    break;
            }
        }

        private void ShowItemIcon(PickableItem item, bool animate)
        {
            if (item == powerCell)
            {
                powerCellUI.ShowIcon(animate);
            }

            if (item == transmitter)
            {
                transmitterUI.ShowIcon(animate);
            }
            
            ManageBarAppearance();
        }

        private void HideItemIcon(PickableItem item)
        {
            if (item == powerCell)
            {
                powerCellUI.HideIcon();
            }

            if (item == transmitter)
            {
                transmitterUI.HideIcon();
            }
        }

        public void OnIconHidden()
        {
            ManageBarAppearance();
        }

        private bool IsIconVisible()
        {
            return powerCellUI.gameObject.activeInHierarchy || transmitterUI.gameObject.activeInHierarchy;
        }

        private void ManageBarAppearance()
        {
            StopAllCoroutines();
            if (IsIconVisible() && !m_barVisible)
            {
                m_barVisible = true;
                StartCoroutine(FadeBar(bar.color.a, fadedInAlpha));
                return;
            }

            if (!IsIconVisible() && m_barVisible)
            {
                m_barVisible = false;
                StartCoroutine(FadeBar(bar.color.a, fadedOutAlpha));
            }
        }

        private IEnumerator FadeBar(float from, float to)
        {
            m_currentFadeDuration = 0;
            var color = bar.color;
            while (m_currentFadeDuration < fadeDuration)
            {
                m_currentFadeDuration += Time.deltaTime;
                var t = Mathf.Clamp01(m_currentFadeDuration / fadeDuration);
                color.a = Mathf.Lerp(from, to, t);
                bar.color = color;
                yield return null;
            }
        }

        private Vector2 GetItemIconScreenPosition(PickableItem item)
        {
            if (item == powerCell)
            {
                var rect = (RectTransform)powerCellUI.transform;
                return rect.position;
            }
            
            else
            {
                var rect = (RectTransform)transmitterUI.transform;
                return rect.position;
            }
        }
    }
}