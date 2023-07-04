using System.Collections.Generic;
using Gameplay.Collectibles;
using SavingSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Characters.CharacterAbilities.Inventory
{
    [CreateAssetMenu(fileName = "Inventory Items", menuName = "ScriptableObjects/Inventory/InventoryItemsContainer", order = 0)]
    public class InventorySlots : ScriptableObject
    {
        public List<PickableItem> itemsInInventory;

        public UnityAction<PickableItem> onItemAdded;
        public UnityAction<PickableItem> onItemRemoved;
        public UnityAction onInitialize;

        public UnityAction onInventoryChanged;
        
        public bool ContainItem(PickableItem pickableItem)
        {
            return itemsInInventory.Contains(pickableItem);
        }

        public void EmptyInventory()
        {
            itemsInInventory.Clear();
            onInitialize?.Invoke();
        }
        
        public void AddItem(PickableItem pickableItem)
        {
            if (ContainItem(pickableItem)) return;
            itemsInInventory.Add(pickableItem);
            onItemAdded?.Invoke(pickableItem);
            onInventoryChanged?.Invoke();
        }

        public void RemoveItem(PickableItem pickableItem)
        {
            if (!ContainItem(pickableItem)) return;
            itemsInInventory.Remove(pickableItem);
            onItemRemoved?.Invoke(pickableItem);
            onInventoryChanged?.Invoke();
        }
    }
}