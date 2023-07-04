namespace Characters.CharacterAbilities.Movement
{
    public interface IHicksMovement : IPlayerMovement
    {
        float StealthMovementSpeed { get; }
        
        bool StealthModeActive { get; set; }
    }
}