using System;
using Characters.CharacterAbilities.AbilitiesPower;
using Characters.CharacterAbilities.Movement.InputCorrection;
using GeneralEnums;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Characters.CharacterAbilities.Boost
{
    public class Boost : MonoBehaviour
    {
        private Rigidbody2D m_rb2d;
        [SerializeField] private FloatVariable lookingDirection;
        private IInputFilter m_inputFilter;
        
        private IAbilitiesPower m_power;

        private bool m_boostInUse;
        
        [SerializeField] private FloatReference boostValue;
        private const int BoostMultiplier = 50;
        
        [SerializeField] private FloatReference boostPowerConsumptionRate;

        [FoldoutGroup("Events")]
        [FoldoutGroup("Events/Listening to")][SerializeField] private VoidEventChannelSO startUsingBoostChannel;
        [FoldoutGroup("Events/Listening to")][SerializeField] private VoidEventChannelSO stopUsingBoostChannel;
        
        [FoldoutGroup("Events/Feedback Events")] public UnityEvent onBoostStart;
        [FoldoutGroup("Events/Feedback Events")] public UnityEvent onBoostEnd;
        
        private void Awake()
        {
            m_rb2d = GetComponent<Rigidbody2D>();
            m_power = GetComponent<IAbilitiesPower>();
            
            m_inputFilter = GetComponent<InputFilter>();
        }

        private void OnEnable()
        {
            startUsingBoostChannel.onEventRaised += StartUsingBoost;
            stopUsingBoostChannel.onEventRaised += StopUsingBoost;
        }

        private void OnDisable()
        {
            startUsingBoostChannel.onEventRaised -= StartUsingBoost;
            stopUsingBoostChannel.onEventRaised -= StopUsingBoost;
            
            StopUsingBoost();
        }

        private void FixedUpdate()
        {
            if (m_boostInUse && !CanUseBoost())
            {
                StopUsingBoost();
                return;
            }

            if (!m_boostInUse || m_power.IsEmpty) return;
            
            m_rb2d.AddForce(MathCalculation.ConvertAngleToDirection(lookingDirection.Value) * (boostValue * BoostMultiplier * Time.deltaTime));
            m_power.ConsumePower(boostPowerConsumptionRate * Time.fixedDeltaTime);
        }

        private bool CanUseBoost()
        {
            return !m_power.IsEmpty && !m_inputFilter.IsDirectionLocked(CardinalDirectionSetter.TransformAngleToCardinal
                (lookingDirection.Value));
        }

        private void StartUsingBoost()
        {
            if (m_boostInUse || !CanUseBoost()) return;
            
            m_boostInUse = true;
            onBoostStart?.Invoke();
        }

        private void StopUsingBoost()
        {
            if (!m_boostInUse) return;
            
            m_boostInUse = false;
            onBoostEnd?.Invoke();
        }
    }
}