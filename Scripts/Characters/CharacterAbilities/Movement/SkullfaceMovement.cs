using Characters.CharacterAbilities.Movement.InputCorrection;
using GeneralScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Characters.CharacterAbilities.Movement
{
	public class SkullfaceMovement : PlayerMovement
    {
	    [FoldoutGroup("Movement Settings")]
	    [SerializeField] private SkullfaceMovementSettings movementSettings;
        
        private const int SpeedMultiplier = 100;

        [SerializeField] private BoolVariable aboveVoid;
        [SerializeField] private BoolVariable platformNearby;
        [SerializeField] private Vector2Variable lastGroundPos;
        
        private float m_returnToPlatformVelocityThreshold;
        
        private IInputFilter m_inputFilter;
        
        protected override void Awake()
        {
			base.Awake();
			
	        m_inputFilter = GetComponent<InputFilter>();
	        m_returnToPlatformVelocityThreshold = movementSettings.returnToPlatformVelocityThreshold * movementSettings.returnToPlatformVelocityThreshold;
        }
        
        private void OnEnable()
        {
	        rb2d.drag = movementSettings.dragOnPlatform;
        }

        private void FixedUpdate()
        {
	        switch (aboveVoid.Value)
	        {
		        case true:
			        ManageReturnToPlatform();
			        break;
		        case false:
			        ManageMovement();
			        break;
	        }
        }

        private void ManageReturnToPlatform()
        {
	        if (platformNearby.Value && rb2d.velocity.sqrMagnitude < m_returnToPlatformVelocityThreshold)
	        {
		        rb2d.AddForce((lastGroundPos.Value - (Vector2)transform.position).normalized *
		                      (movementSettings.returnToPlatformSpeed * SpeedMultiplier * Time.fixedDeltaTime));
	        }
        }

        protected override void ManageMovement()
        {
	        if (movementInput.Value.sqrMagnitude < movementSettings.detectInputThreshold) return;
	        
	        movementAmplitude = m_inputFilter.CorrectInput(movementInput.Value);
	        rb2d.AddForce(movementAmplitude * (SpeedMultiplier * (movementSettings.movementSpeed * Time.fixedDeltaTime)));
	        ChangeLookingDirection(MathCalculation.ConvertDirectionToAngle(movementInput.Value.normalized));
        }

        public void SetPlatformDrag()
        {
	        rb2d.drag = movementSettings.dragOnPlatform;
        }

        public void SetVoidDrag()
        {
	        rb2d.drag = movementSettings.dragInVoid;
        }
    }
}