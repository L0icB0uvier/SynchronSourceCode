using UnityEngine;

namespace Gameplay.InteractionSystem.Interacters
{
    [CreateAssetMenu(menuName = "Gameplay/Interacter Profile")]
    public class InteracterProfileSO : ScriptableObject
    {
        public string interacterName;
        
        public enum EInteracterActionType{ShowNoActionText, ShowHicksActionText, ShowSkullfaceActionText}

        public EInteracterActionType interacterActionType;
    }
}