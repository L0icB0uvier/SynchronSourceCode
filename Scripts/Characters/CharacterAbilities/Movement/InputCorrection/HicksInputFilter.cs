using System.Linq;
using GeneralEnums;
using GeneralScriptableObjects;
using UnityEngine;

namespace Characters.CharacterAbilities.Movement.InputCorrection
{
    public class HicksInputFilter : InputFilter
    {
        [SerializeField] private CharacterDirectionLocker[] directionLockers;

        public override bool IsDirectionLocked(EIsometricCardinal4DiagonalDirection direction)
        {
            return directionLockers.Any(x => x.LockedDirectionInfo[direction]);
        }
    }
}