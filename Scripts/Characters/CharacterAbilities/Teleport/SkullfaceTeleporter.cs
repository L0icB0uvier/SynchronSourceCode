using Characters.CharacterAbilities.Teleport.Resolve;
using GeneralScriptableObjects;
using UnityEngine;
using Utilities;

namespace Characters.CharacterAbilities.Teleport
{
    public class SkullfaceTeleporter : Teleporter
    {
        [SerializeField] private BoolVariableNotifyChange skullfaceMovementOverride;
        
        protected override void Initialise()
        {
            teleportDestinationTransform = FindObjectOfType<HicksTeleporter>().transform;
            
            var skullfaceResolver = Instantiate(PrefabInstantiationUtility.GetGameObjectRefByName("SkullfaceResolver"));
            resolver = skullfaceResolver.GetComponent<IResolver>();
            initialised = true;
        }

        protected override bool IsTeleportationPossible()
        {
            var destinationLocation = teleportDestinationTransform.position;
            var departureLocation = transform.position;
            return !skullfaceMovementOverride.Value && !EnvironmentalQueryUtilities.IsInsideJammer(destinationLocation) && 
                   !EnvironmentalQueryUtilities.IsInsideJammer(departureLocation) && 
                   EnvironmentalQueryUtilities.IsOnGround(destinationLocation) && 
                   !EnvironmentalQueryUtilities.IsSightBlockedByObstacle(departureLocation, destinationLocation);
        }
    }
}