using UnityEngine;

namespace Characters.CharacterAbilities.Movement
{
    [CreateAssetMenu(fileName = "SkullfaceMovementSettings", menuName = "ScriptableObjects/MovementSettings/Skullface", order = 0)]
    public class SkullfaceMovementSettings : ScriptableObject
    {
        public float movementSpeed = 30;
        public float dragOnPlatform = 10;
        public float dragInVoid = 6;
        public float detectInputThreshold = .5f;
        public float returnToPlatformVelocityThreshold = 3;
        public float returnToPlatformSpeed = 15;
    }
}