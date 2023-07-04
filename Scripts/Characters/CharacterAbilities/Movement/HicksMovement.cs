using System;
using Characters.CharacterAbilities.Movement.InputCorrection;
using GeneralScriptableObjects;
using Sirenix.OdinInspector;
using Tools.Extension;
using UnityEngine;
using Utilities;

namespace Characters.CharacterAbilities.Movement
{
    public class HicksMovement : PlayerMovement
    {
	    private const int MovementMultiplier = 1000;

	    [FoldoutGroup("MovementSettings")][PropertyOrder(0)]
	    [SerializeField] private HicksMovementSettings movementSettings;
	    
	    [FoldoutGroup("Tracked Variables")]
	    [SerializeField] private BoolVariable stealthModeActive;

	    private IInputFilter m_inputFilter;
	    
        protected override void Awake()
        {
	        base.Awake();
	        
	        m_inputFilter = GetComponent<InputFilter>();
        }

        private void OnEnable()
        {
	        rb2d.drag = movementSettings.drag;
        }

        private void OnDisable()
        {
	        isMoving.SetValue(false);
        }

        private void FixedUpdate()
        {
	        if (movementInput.Value.sqrMagnitude < movementSettings.detectInputThreshold) return;
	        
	        if(isMoving.Value) ChangeLookingDirection(MathCalculation.ConvertDirectionToAngle(rb2d.velocity.normalized));

	        ManageMovement();
        }

        protected override void ManageMovement()
        {
	        movementAmplitude = m_inputFilter.CorrectInput(movementInput.Value);

	        var addedForce = movementAmplitude * (GetMovementSpeed() * MovementMultiplier * MovementExtension
	        .GetVerticalSpeedModifier(Mathf.Abs
		        (movementInput.Value.y)) * Time.fixedDeltaTime);

	        rb2d.AddForce(addedForce);
        }

        private float GetMovementSpeed()
        {
	        return stealthModeActive.Value ? movementSettings.stealthMovementSpeed : movementSettings.defaultMovementSpeed;
        }
    }
}