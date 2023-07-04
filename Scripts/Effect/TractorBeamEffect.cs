using System;
using GeneralScriptableObjects;
using UnityEngine;
using Utilities;

namespace Effect
{
    public class TractorBeamEffect : MonoBehaviour
    {
        [SerializeField] private LineRenderer beamLineRenderer;
        [SerializeField] private ParticleSystem beamParticleSystem;
        private ParticleSystem.MainModule m_mainModule;

        [SerializeField] private Transform endTransform;
        [SerializeField] private FloatVariable yOffsetPos;

        private void Awake()
        {
            endTransform = GameObject.FindGameObjectWithTag("Hicks").transform;
            m_mainModule = beamParticleSystem.main;
        }

        private void OnEnable()
        {
            UpdateBeamPosition();
        }

        private void Update()
        {
            UpdateBeamPosition();
        }

        private void UpdateBeamPosition()
        {
            var position = transform.position;
            var endPosition = endTransform.position;
            
            beamLineRenderer.SetPosition(0, position + new Vector3(0, yOffsetPos.Value, 0));
            beamLineRenderer.SetPosition(1, endPosition + new Vector3(0, yOffsetPos.Value, 0));

            var angle = MathCalculation.GetAngleBetween2Points(position, endPosition);

            var particleTransform = beamParticleSystem.transform;
            particleTransform.localPosition = new Vector3(0, yOffsetPos.Value, 0);
            particleTransform.eulerAngles = new Vector3(0, 0, angle);

            var distance = Vector2.Distance(position, endPosition);

            m_mainModule.startLifetimeMultiplier = distance / m_mainModule.startSpeedMultiplier;
        }
    }
}