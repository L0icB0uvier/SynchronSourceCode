using Characters.Controls.Controllers;
using UnityEngine;
using Utilities;

namespace Characters.CharacterAbilities.Teleport.TeleportationConditionCheck
{
    public class SkullfaceTeleportConditionChecker : ITeleportConditionChecker
    {
        private readonly IController m_controller;

        public SkullfaceTeleportConditionChecker(IController controller)
        {
            m_controller = controller;
        }
        
        public bool IsTeleportationPossible(Vector2 teleportDepartureLocation, Vector2 teleportDestinationLocation)
        {
            return !EnvironmentalQueryUtilities.IsInsideJammer(teleportDestinationLocation) && 
                   !EnvironmentalQueryUtilities.IsInsideJammer(teleportDepartureLocation) && 
                   EnvironmentalQueryUtilities.IsOnGround(teleportDestinationLocation) && 
                   !EnvironmentalQueryUtilities.IsSightBlockedByObstacle(teleportDepartureLocation, teleportDestinationLocation);
        }
    }
}