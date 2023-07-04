using UnityEngine;

namespace Characters.CharacterAbilities.Teleport.Resolve
{
    public interface IResolver
    {
        void Initialise(Vector2 resolvePos);

        void UpdateResolveValue(float value);

        void Hide();
    }
}