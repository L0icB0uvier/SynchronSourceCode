using UnityEngine;

namespace Characters.CharacterAbilities.Teleport.TeleportationConditionCheck
{
    public interface ITeleportConditionChecker
    {
        bool IsTeleportationPossible(Vector2 teleportDepartureLocation, Vector2 teleportDestinationLocation);
    }
}