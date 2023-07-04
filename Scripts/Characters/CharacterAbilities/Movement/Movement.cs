using GeneralScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.CharacterAbilities.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Movement : MonoBehaviour
    {
        protected Rigidbody2D rb2d;

        [FoldoutGroup("Tracked Variables")][PropertyOrder(1)]
        [SerializeField] private FloatVariable lookingDirection;

        protected virtual void Awake()
        {
            rb2d = GetComponent<Rigidbody2D>();
        }

        protected abstract void ManageMovement();

        protected void ChangeLookingDirection(float newLookingDirection)
        {
            lookingDirection.SetValue(newLookingDirection);
        }
    }
}