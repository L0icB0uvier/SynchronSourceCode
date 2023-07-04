using UnityEngine;

namespace UI
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/TeleportationUISettings", order = 0)]
    public class TeleportationUISettings : ScriptableObject
    {
        public float showUIMinDistance = 4;

        public float defaultWidthMultiplier = .5f;
        public float teleportationFailedFeedbackDuration = 1;
        public float teleportFailedWidthMultiplier = 1;
        public float teleportFailedAlpha;
        
        public float lineStartVisualOffset = 2;
        public float tpVisualYOffset = 1;
        public Color tpPossibleColor;
        public Color tpImpossibleColor;
        public float fadeSpeed = 1;
        public float fadeMinAlpha;
        public float fadeMaxAlpha = .5f;
        
        public LayerMask teleportObstacleLayerMask;
    }
}