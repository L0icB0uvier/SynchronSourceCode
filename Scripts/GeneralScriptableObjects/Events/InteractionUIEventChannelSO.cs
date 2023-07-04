using Characters.Controls.Controllers.PlayerControllers;
using UI.CallToActionUI;
using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/ActionUIEvents/Interaction UI Event Channel")]
    public class InteractionUIEventChannelSO : ScriptableObject
    {
        public UnityAction<EPlayerCharacterType, StringVariable, ActionUISettings> OnEventRaised;
	
        public void RaiseEvent(EPlayerCharacterType characterType, StringVariable text, ActionUISettings 
        actionUISetting)
        {
            OnEventRaised?.Invoke(characterType, text, actionUISetting);
        }
    }
}