using Sirenix.OdinInspector;
using UnityEngine;

namespace SceneManagement.SceneTransition
{
    [CreateAssetMenu(fileName = "ExitLocationBarkSettings", menuName = "ScriptableObjects/Settings/Barks/ExitLocationBarkSettings", order = 0)]
    public class ExitLocationBarkSettings : ScriptableObject
    {
        public bool randomDelay;

        [Range(0,10)][HideIf("randomDelay")]
        public float delay = 2;
        
        [MinMaxSlider(0, 10)][ShowIf("randomDelay")]
        public Vector2 delayRange = new Vector2(1, 3);

        public bool playTransitionImpossibleBark;
        public EExitBarkType exitBarkType;

        public bool repeatBark;
        public bool repeatOnCharacterStay;

        public float delayBeforeRepeat;
        
        public enum EExitBarkType {CharacterInTrigger, OtherCharacter, Random}
    }
}