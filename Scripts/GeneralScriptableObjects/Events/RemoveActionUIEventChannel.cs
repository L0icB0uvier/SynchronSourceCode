using Characters.Controls.Controllers.PlayerControllers;
using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/ActionUIEvents/Remove action UI Event Channel")]
    public class RemoveActionUIEventChannel : ScriptableObject
    {
        public UnityAction<EPlayerCharacterType, StringVariable> OnEventRaised;
	
        public void RaiseEvent(EPlayerCharacterType characterType, StringVariable text)
        {
            OnEventRaised?.Invoke(characterType, text);
        }
    }
}