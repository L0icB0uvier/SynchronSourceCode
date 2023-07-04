using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/UI/ShowGameOverScreen", order = 0)]
    public class ShowGameOverEventChannel : ScriptableObject
    {
        public UnityAction<Sprite, bool> OnEventRaised;
	
        public void RaiseEvent(Sprite characterSprite, bool showCharacterSprite)
        {
            OnEventRaised?.Invoke(characterSprite, showCharacterSprite);
        }
    }
}