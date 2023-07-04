using System;
using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/Move character to location Event Channel")]
    public class MoveCharacterToPositionChannelSO : ScriptableObject
    {
        public event UnityAction<Vector2, UnityAction> OnEventRaised;

        public void RaiseEvent(Vector2 value, UnityAction callback)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(value, callback);
        }
    }
}