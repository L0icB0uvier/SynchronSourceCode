using System;
using System.Collections.Generic;
using GeneralEnums;
using UnityEngine;

namespace GeneralScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MovementSettings/DirectionLock", order = 0)]
    public class CharacterDirectionLocker : ScriptableObject
    {
        public Dictionary<EIsometricCardinal4DiagonalDirection, bool> LockedDirectionInfo { get; } = new 
            Dictionary<EIsometricCardinal4DiagonalDirection, bool>
            {
                { EIsometricCardinal4DiagonalDirection.NorthEast, false },
                { EIsometricCardinal4DiagonalDirection.NorthWest, false },
                { EIsometricCardinal4DiagonalDirection.SouthWest, false },
                { EIsometricCardinal4DiagonalDirection.SouthEast, false }
            };

        private void OnEnable()
        {
            Reset();
        }

        public void UpdateDirectionLock(EIsometricCardinal4DiagonalDirection direction, bool locked)
        {
            LockedDirectionInfo[direction] = locked;
        }

        public void LockUniqueDirection(EIsometricCardinal4DiagonalDirection direction)
        {
            Reset();
            LockedDirectionInfo[direction] = true;
        }
        
        public void Reset()
        {
            foreach (EIsometricCardinal4DiagonalDirection cardinalDirection in Enum.GetValues(
                typeof(EIsometricCardinal4DiagonalDirection)))
            {
                LockedDirectionInfo[cardinalDirection] = false;
            }
        }
    }
}