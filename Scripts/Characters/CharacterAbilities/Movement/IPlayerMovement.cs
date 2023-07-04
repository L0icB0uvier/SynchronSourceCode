using UnityEngine;

namespace Characters.CharacterAbilities.Movement
{
    public interface IPlayerMovement : IMovement
    {
        public Vector2 MovementInput { get; set; }
    }
}