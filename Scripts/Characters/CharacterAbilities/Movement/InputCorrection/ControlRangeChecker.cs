using System.Diagnostics.CodeAnalysis;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using UnityEngine;
using Utilities;

namespace Characters.CharacterAbilities.Movement.InputCorrection
{
    public class ControlRangeChecker : MonoBehaviour
    {
        [SerializeField] private FloatReference controlRangeDistance;
        
        [SerializeField] [SuppressMessage("ReSharper", "InconsistentNaming")]
        private InputFilter[] objectToConstraint = new InputFilter[2];

        [SerializeField] private float glitchStartDistanceFactor;
        
        public float outRangeGlitchStrength = .5f;

        [SerializeField] private CharacterDirectionLocker hicksDirectionLocker;
        [SerializeField] private CharacterDirectionLocker skullfaceDirectionLocker;
        
        [SerializeField] private VoidEventChannelSO enableGlitchChannel;
        [SerializeField] private VoidEventChannelSO disableGlitchChannel;

        [SerializeField] private ChangeCameraGlitchChannelSO changeCameraGlitchSettingChannel;
        
        [SerializeField] private VoidEventChannelSO[] enableRangeControllerChannel;
        [SerializeField] private VoidEventChannelSO[] disableRangeControllerChannel;

        private bool m_outOfRange;
        
        private float m_distanceFactor;
        private bool m_inGlitchRange;
        
        private void Awake()
        {
            objectToConstraint[0] = FindObjectOfType<HicksInputFilter>();
            objectToConstraint[1] = FindObjectOfType<SkullfaceInputFilter>();
            
            foreach (var channel in enableRangeControllerChannel)
            {
                channel.onEventRaised += EnableRangeController;
            }
	        
            foreach (var channel in disableRangeControllerChannel)
            {
                channel.onEventRaised += DisableRangeController;
            }
        }

        private void OnDestroy()
        {
            foreach (var channel in enableRangeControllerChannel)
            {
                channel.onEventRaised -= EnableRangeController;
            }
	        
            foreach (var channel in disableRangeControllerChannel)
            {
                channel.onEventRaised -= DisableRangeController;
            }
        }

        private void FixedUpdate()
        {
            var midPos = MathCalculation.GetMiddlePositionBetween2Points(objectToConstraint[0].ObjectTransform.position,
            objectToConstraint[1].ObjectTransform.position);
            
            if (HasReachedControlRangeLimit(midPos))
            {
                m_outOfRange = true;
                var hicksAngle = MathCalculation.GetAngleBetween2Points(midPos,objectToConstraint[0].ObjectTransform.position);
                var hicksCardinalDir = CardinalDirectionSetter.TransformAngleToCardinal(hicksAngle);
                hicksDirectionLocker.LockUniqueDirection(hicksCardinalDir);

                var skullfaceAngle = MathCalculation.GetAngleBetween2Points(midPos,objectToConstraint[1].ObjectTransform.position);
                var skullfaceCardinalDir = CardinalDirectionSetter.TransformAngleToCardinal(skullfaceAngle);
                skullfaceDirectionLocker.LockUniqueDirection(skullfaceCardinalDir);
            }

            else if(m_outOfRange)
            {
                hicksDirectionLocker.Reset();
                skullfaceDirectionLocker.Reset();
                m_outOfRange = false;
            }
            
            ManageGlitch();
        }

        private void ManageGlitch()
        {
            if (m_distanceFactor > glitchStartDistanceFactor)
            {
                if (!m_inGlitchRange)
                {
                    enableGlitchChannel.RaiseEvent();
                    m_inGlitchRange = true;
                }

                UpdateGlitch();
            }

            else if (m_inGlitchRange)
            {
                disableGlitchChannel.RaiseEvent();
                m_inGlitchRange = false;
            }
        }

        private bool HasReachedControlRangeLimit(Vector2 midPos)
        {
            var objectPos = (Vector2)objectToConstraint[0].ObjectTransform.position;
            var direction = MathCalculation.GetDirectionalVectorBetween2Points(midPos, objectPos);
            var rangeLimit =  MathCalculation.GetPointOnEllipse(midPos, controlRangeDistance, direction);
            var sqrDistanceToPos = (objectPos - midPos).sqrMagnitude;
            var sqrDistanceToRangeLimit = (rangeLimit - midPos).sqrMagnitude;
            m_distanceFactor = Mathf.Clamp01(sqrDistanceToPos / sqrDistanceToRangeLimit);
            
            return sqrDistanceToPos >= sqrDistanceToRangeLimit;
        }

        private void UpdateGlitch()
        {
            var glitchFactor = MathCalculation.Remap(m_distanceFactor, glitchStartDistanceFactor, 1, 0, outRangeGlitchStrength);

            changeCameraGlitchSettingChannel.RaiseEvent(EGlitchSettingType.Rate, 1 - glitchFactor);
            changeCameraGlitchSettingChannel.RaiseEvent(EGlitchSettingType.IntensityShift, glitchFactor);
        }

        private void EnableRangeController()
        {
            enabled = true;
        }

        private void DisableRangeController()
        {
            enabled = false;
        }
    }
}

public enum EGlitchSettingType { Rate, IntensityShift, Interval, NoiseIntensity}