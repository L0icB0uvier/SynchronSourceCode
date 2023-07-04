using Gameplay.Collectibles;

namespace Characters.CharacterAbilities.Inventory
{
    public interface IInventory
    {
        bool ContainItem(PickableItem pickableItem);
        
        bool IsEmpty { get; }

        void AddItem(PickableItem pickableItem);
        void RemoveItem(PickableItem pickableItemType);
    }
}