using UnityEngine;

namespace Characters.CharacterAbilities.Movement
{
    [CreateAssetMenu(fileName = "HicksMovementSettings", menuName = "ScriptableObjects/MovementSettings/Hicks", order = 0)]
    public class HicksMovementSettings : ScriptableObject
    {
        public float defaultMovementSpeed = 20;
        public float stealthMovementSpeed = 8;
        public float detectInputThreshold = .5f;
        public float drag = 30;
    }
}