using GeneralScriptableObjects.SceneData;
using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/Location Changed Event Channel")]
    public class LocationChangeChannelSO : ScriptableObject
    {
        public UnityAction<LocationSO> OnLoadingRequested;

        public void RaiseEvent(LocationSO locationToLoad)
        {
            OnLoadingRequested?.Invoke(locationToLoad);
        }
    }
}