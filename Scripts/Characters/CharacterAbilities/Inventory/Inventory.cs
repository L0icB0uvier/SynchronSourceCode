using Gameplay.Collectibles;
using UnityEngine;

namespace Characters.CharacterAbilities.Inventory
{
    public class Inventory : MonoBehaviour, IInventory
    {
        [SerializeField] private InventorySlots slots;

        public void EmptyInventory()
        {
            slots.EmptyInventory();
        }
        
        public bool ContainItem(PickableItem pickableItem)
        {
            return slots.ContainItem(pickableItem);
        }

        public bool IsEmpty => slots.itemsInInventory.Count == 0;

        public void AddItem(PickableItem pickableItem)
        {
            slots.AddItem(pickableItem);
        }

        public void RemoveItem(PickableItem pickableItem)
        {
            slots.RemoveItem(pickableItem);
        }
    }
}