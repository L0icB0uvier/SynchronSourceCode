using System;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Characters.CharacterAbilities.Movement
{
    public abstract class PlayerMovement : Movement
    {
        [FoldoutGroup("Tracked Variables")][PropertyOrder(1)]
        [SerializeField] protected Vector2Variable movementInput;
        
        [FoldoutGroup("Tracked Variables")]
        [SerializeField] protected BoolVariable isMoving;
        
        protected Vector2 movementAmplitude;

        [FoldoutGroup("Events")] public UnityEvent onStartMoving;
        [FoldoutGroup("Events")] public UnityEvent onStopMoving;
        
        private void Start()
        {
            isMoving.SetValue(false);
        }

        private void Update()
        {
            bool previousIsMoving = isMoving.Value;
            isMoving.SetValue(rb2d.velocity.sqrMagnitude > 0.1f);

            if (previousIsMoving != isMoving.Value)
            {
                if(isMoving.Value) onStartMoving?.Invoke();
                else onStopMoving?.Invoke();
            }
        }
    }
}