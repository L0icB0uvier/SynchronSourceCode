using System.Linq;
using GeneralEnums;
using GeneralScriptableObjects;
using UnityEngine;

namespace Characters.CharacterAbilities.Movement.InputCorrection
{
    public class SkullfaceInputFilter : InputFilter
    {
        [SerializeField] private CharacterDirectionLocker[] directionLocker;
        
        public override bool IsDirectionLocked(EIsometricCardinal4DiagonalDirection direction)
        {
            return directionLocker.Any(x => x.LockedDirectionInfo[direction]);
        }
    }
}