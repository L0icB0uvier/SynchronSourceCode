using Characters.CharacterAbilities.Inventory;
using Characters.Controls.Controllers;
using UnityEngine;
using Utilities;

namespace Characters.CharacterAbilities.Teleport.TeleportationConditionCheck
{
    public class HicksTeleportConditionChecker : ITeleportConditionChecker
    {
        private readonly IInventory m_inventory;

        private readonly IController m_controller;

        public HicksTeleportConditionChecker(IInventory inventory, IController controller)
        {
            m_inventory = inventory;
            m_controller = controller;
        }

        public bool IsTeleportationPossible(Vector2 teleportDepartureLocation, Vector2 teleportDestinationLocation)
        {
            return m_inventory.IsEmpty && !EnvironmentalQueryUtilities.IsInsideJammer(teleportDestinationLocation) && 
                   !EnvironmentalQueryUtilities.IsInsideJammer(teleportDepartureLocation) && EnvironmentalQueryUtilities.IsOnGround(teleportDestinationLocation) && 
                   !EnvironmentalQueryUtilities.IsSightBlockedByObstacle(teleportDepartureLocation, teleportDestinationLocation);
        }
    }
}