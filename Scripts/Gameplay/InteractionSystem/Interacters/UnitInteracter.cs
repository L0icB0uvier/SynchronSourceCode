namespace Gameplay.InteractionSystem.Interacters
{
    public class UnitInteracter : Interacter
    {
        public override void TryInteraction()
        {
            connectionInterface.TryInteraction();
        }
    }
}