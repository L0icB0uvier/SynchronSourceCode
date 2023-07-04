using Gameplay.InteractionSystem.Interacters;

namespace Gameplay.InteractionSystem.Interactables
{
   public interface IInteractable
   {
      public bool TryInteraction(Interacter interacter);

      public InteracterProfileSO[] AuthorizedInteracters { get; }
   }
}
