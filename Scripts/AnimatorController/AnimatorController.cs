using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AnimatorController
{
    public abstract class AnimatorController : MonoBehaviour
    {
        [SerializeField] protected Animator animator;

        protected virtual void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }
        
        public void ResetTriggerParameters()
        {
            foreach (var param in animator.parameters)
            {
                if(param.type == AnimatorControllerParameterType.Trigger)
                {
                    animator.ResetTrigger(param.name);
                }
            }
        }
        
        public void InitializeAnimator()
        {
            animator.Rebind();
        } 
    }
}