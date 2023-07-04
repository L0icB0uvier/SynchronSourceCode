using UnityEngine;

namespace Gameplay.Collectibles
{
    [CreateAssetMenu(fileName = "Pickable Item", menuName = "ScriptableObjects/Inventory/PickableItem")]
    public class PickableItem : ScriptableObject
    {
        public Sprite collectibleIcon;
    }
}