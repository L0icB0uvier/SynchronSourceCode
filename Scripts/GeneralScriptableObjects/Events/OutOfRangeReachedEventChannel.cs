using GeneralEnums;
using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/Out Of Range Channel")]
    public class OutOfRangeReachedEventChannel : ScriptableObject
    {
        public event UnityAction<EIsometricCardinal4DiagonalDirection> OnEventRaised;

        public void RaiseEvent(EIsometricCardinal4DiagonalDirection direction)
        {
            OnEventRaised?.Invoke(direction);
        }
    }
}