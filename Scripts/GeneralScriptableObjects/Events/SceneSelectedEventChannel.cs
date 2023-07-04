using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(fileName = "SceneSelectedEventChannel", menuName = "Events/SceneSelectedEventChannel", order = 0)]
    public class SceneSelectedEventChannel : ScriptableObject
    {
        public UnityAction<LocationSO, PathSO> OnEventRaised;
	
        public void RaiseEvent(LocationSO location, PathSO path)
        {
            OnEventRaised?.Invoke(location, path);
        }
    }
}