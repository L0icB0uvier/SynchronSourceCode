using System;
using System.Collections;
using Audio;
using Characters.CharacterAbilities.Inventory;
using Characters.CharacterAbilities.Teleport.Resolve;
using Characters.CharacterAbilities.Teleport.TeleportationConditionCheck;
using Characters.Controls.Controllers;
using Characters.Controls.Controllers.PlayerControllers;
using Characters.Controls.Input;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using Lean.Pool;
using NoiseSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Characters.CharacterAbilities.Teleport
{
    public abstract class Teleporter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Vector2 m_tpLocation;
        
        public float teleportationLenght = .5f;
        
        [SerializeField] private NoiseEmissionProfile noiseEmissionProfile;
        
        protected IResolver resolver;

        protected Transform teleportDestinationTransform;
        
        private static readonly int dissolvePower = Shader.PropertyToID("_DissolvePower");

        [SerializeField] private GenerateNoiseChannel generateNoiseChannel;
        
        [FoldoutGroup("Events")]
        [FoldoutGroup("Events/Listening to")][SerializeField] private VoidEventChannelSO _playerCharactersSpawnedChannel;
        [FoldoutGroup("Events/Listening to")][SerializeField] private VoidEventChannelSO teleportChannel;
        
        [FoldoutGroup("Events/Broadcasting on")][SerializeField] private VoidEventChannelSO teleportStartChannel;
        [FoldoutGroup("Events/Broadcasting on")][SerializeField] private VoidEventChannelSO teleportEndChannel;
        [FoldoutGroup("Events/Broadcasting on")] public VoidEventChannelSO teleportFailedChannel;

        [FoldoutGroup("Events/UnityEvents")] public UnityEvent onTeleportStart;
        [FoldoutGroup("Events/UnityEvents")] public UnityEvent onTeleportEnd;
        [FoldoutGroup("Events/UnityEvents")] public UnityEvent onTeleportFailed;

        [SerializeField] private BoolVariable isTeleportationPossible;

        [SerializeField] private BoolVariable teleportInProgress;

        protected bool initialised;
        
        private void OnEnable()
        {
            _playerCharactersSpawnedChannel.onEventRaised += Initialise;
            teleportChannel.onEventRaised += TryTeleport;
            teleportInProgress.SetValue(false);
        }

        private void OnDisable()
        {
            _playerCharactersSpawnedChannel.onEventRaised -= Initialise;
            teleportChannel.onEventRaised -= TryTeleport;
            teleportInProgress.SetValue(false);
        }

        protected abstract void Initialise();

        private void FixedUpdate()
        {
            if (!initialised) return;
            isTeleportationPossible.Value = IsTeleportationPossible();
        }

        protected abstract bool IsTeleportationPossible();
        
        public void TryTeleport()
        {
            if (!isTeleportationPossible.Value)
            {
                onTeleportFailed?.Invoke();
                teleportFailedChannel.RaiseEvent();
                return;
            }

            teleportStartChannel.RaiseEvent();

            m_tpLocation = teleportDestinationTransform.position;
            resolver.Initialise(m_tpLocation);
            StartCoroutine(EngageTeleport());
        }
        
        public void Teleport()
        {
            transform.position = m_tpLocation;
            spriteRenderer.material.SetFloat(dissolvePower, 1);
            resolver.Hide();
            generateNoiseChannel.RaiseEvent(transform.position, noiseEmissionProfile.noiseAmplitude, noiseEmissionProfile.stoppedByWalls, ENoiseInstigator.Player);
            LeanPool.Spawn(PrefabInstantiationUtility.GetGameObjectRefByName("Dust"), transform.position, Quaternion.identity);
            teleportInProgress.SetValue(false);
            teleportEndChannel.RaiseEvent();
            onTeleportEnd?.Invoke();
        }

        public IEnumerator EngageTeleport()
        {
            onTeleportStart?.Invoke();
            teleportInProgress.SetValue(true);
            float currentDissolveTime = teleportationLenght;
            bool dissolving = true;

            while (dissolving)
            {
                currentDissolveTime = Mathf.Clamp(currentDissolveTime - Time.fixedDeltaTime, 0, teleportationLenght);

                float dissolveValue = currentDissolveTime / teleportationLenght;

                UpdateShaders(dissolveValue);

                if (currentDissolveTime == 0)
                {
                    dissolving = false;
                }

                else
                {
                    yield return new WaitForFixedUpdate();
                }
            }

            Teleport();
        }

        private void UpdateShaders(float dissolveValue)
        {
            spriteRenderer.material.SetFloat(dissolvePower, dissolveValue);
            resolver.UpdateResolveValue(1 - dissolveValue);
        }
    }
}