using System.Collections;
using UnityEngine.Events;

namespace Characters.CharacterAbilities.Fall
{
    public interface IFall
    {
        bool Falling { get; }

        void StartFalling();

        UnityEvent OnStartFalling { get; }
        
        IEnumerator Fall();
    }
}