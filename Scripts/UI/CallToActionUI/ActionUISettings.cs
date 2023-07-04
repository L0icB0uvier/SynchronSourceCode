using GeneralEnums;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.CallToActionUI
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/ActionUISetting", order = 0)]
    public class ActionUISettings : ScriptableObject
    {
        public EActionDisplayType actionDisplayType;
        
        [FormerlySerializedAs("orientation")] [ShowIf("ShowDirection")]
        public EIsometricCardinal8Direction direction;

        [ShowIf("ShowDirection")]
        public bool iconOnTheRight;
        
        [Range(0, 10)]
        public float distanceFromParent;

        private bool ShowDirection()
        {
            return actionDisplayType == EActionDisplayType.Direction;
        }

        public enum EActionDisplayType
        {
            CenteredTop,
            Direction,
        }
    }
}