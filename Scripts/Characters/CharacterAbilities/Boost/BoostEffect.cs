using System;
using Characters.CharacterAbilities.Movement;
using GeneralScriptableObjects;
using UnityEngine;

namespace Characters.CharacterAbilities.Boost
{
    public class BoostEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem boostParticleSystem;
        private ParticleSystem.ShapeModule m_boostShapeModule;
        
        [SerializeField] private TrailRenderer boostTrailRenderer;

        private IMovement m_movement;
        [SerializeField] private FloatVariable lookingDirection;
        

        private void Awake()
        {
            m_boostShapeModule = boostParticleSystem.shape;
            m_movement = GetComponentInParent<IMovement>();
        }

        private void Start()
        {
            StopEffect();
        }

        private void Update()
        {
            m_boostShapeModule.rotation = new Vector3(0, 0, lookingDirection.Value + 160);
        }

        public void PlayEffect()
        {
            boostParticleSystem.Play();
            boostTrailRenderer.emitting = true;
        }

        public void StopEffect()
        {
            boostParticleSystem.Stop();
            boostTrailRenderer.emitting = false;
        }
    }
}