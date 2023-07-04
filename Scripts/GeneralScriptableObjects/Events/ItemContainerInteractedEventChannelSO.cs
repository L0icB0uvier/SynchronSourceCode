using Gameplay.Collectibles;
using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/ItemContainerInteractedEventChannel", order = 0)]
    public class ItemContainerInteractedEventChannelSO : ScriptableObject
    {
        public UnityAction<ItemContainer> OnEventRaised;
	
        public void RaiseEvent(ItemContainer value)
        {
            OnEventRaised?.Invoke(value);
        }
    }
}