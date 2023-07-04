using GeneralScriptableObjects;
using UnityEngine;

namespace AnimatorController
{
    public abstract class PlayerAnimatorController : AnimatorController
    {
        [SerializeField] private FloatVariable lookingDirection;
        private static readonly int direction = Animator.StringToHash("Direction");
        
        private void Update()
        {
            SetAnimatorParameters();
        }

        protected virtual void SetAnimatorParameters()
        {
            animator.SetFloat(direction, lookingDirection.Value);
        }
    }
}