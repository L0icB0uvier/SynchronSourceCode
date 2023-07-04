using GeneralEnums;

namespace Utilities
{
    public static class CardinalDirectionSetter
    {
        public static EIsometricCardinal4DiagonalDirection TransformAngleToCardinal(float angle)
        {
            if (angle < 90) return EIsometricCardinal4DiagonalDirection.NorthEast;
            if (angle >= 90 && angle < 180) return EIsometricCardinal4DiagonalDirection.NorthWest;
            if (angle >= 180 && angle < 270) return EIsometricCardinal4DiagonalDirection.SouthWest;
            return EIsometricCardinal4DiagonalDirection.SouthEast;
        }
    }
}