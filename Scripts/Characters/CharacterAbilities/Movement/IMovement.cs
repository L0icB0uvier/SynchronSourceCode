namespace Characters.CharacterAbilities.Movement
{
    public interface IMovement
    {
       float MovementSpeed { get;}
       
       void ChangeLookingDirection(float newLookingDirection);

       void ManageMovement();
    }
}