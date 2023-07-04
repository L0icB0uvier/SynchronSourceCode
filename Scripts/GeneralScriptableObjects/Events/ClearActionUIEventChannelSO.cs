using Characters.Controls.Controllers.PlayerControllers;
using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/ActionUIEvents/Clear Action UI Event Channel")]
    public class ClearActionUIEventChannelSO : ScriptableObject
    {
        public UnityAction<EPlayerCharacterType> onEventRaised;
	
        public void RaiseEvent(EPlayerCharacterType characterType)
        {
            onEventRaised?.Invoke(characterType);
        }
    }
}