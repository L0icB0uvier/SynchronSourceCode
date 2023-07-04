using GeneralEnums;
using UnityEngine;

namespace Characters.CharacterAbilities.Movement.InputCorrection
{
    public interface IInputFilter
    {
        Vector2 CorrectInput(Vector2 input);

        bool IsDirectionLocked(EIsometricCardinal4DiagonalDirection direction);
    }
}