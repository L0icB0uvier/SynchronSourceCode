using Characters.Controls.Controllers.PlayerControllers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.InputTutorial
{
    [CreateAssetMenu(fileName = "InputIconInfo", menuName = "ScriptableObjects/Tutorial/Input Icon Info", order = 0)]
    public class InputIconInfoSO : ScriptableObject
    {
        public bool showPrefixText;
        [ShowIf("showPrefixText")]public string prefixText;
        public Sprite iconSprite;
        public bool showActionName;
        [ShowIf("showActionName")] public string actionText;
        public EPlayerCharacterType aboveCharacter;
    }
}