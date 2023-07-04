using System;
using Characters.Controls.Controllers.AIControllers;
using Characters.Controls.Controllers.AIControllers.Enemies;
using GeneralScriptableObjects.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace SavingSystem
{
    public class EnemyRespawner : Respawner
    {
        private Vector2 m_respawnLocation;
        private float m_respawnLookingDirection;

        private EnemyAIController m_enemyAIController;

        [SerializeField] private VoidEventChannelSO respawnEnemyChannel;

        public UnityEvent onRespawn;
        
        protected override void Awake()
        {
            base.Awake();
            m_enemyAIController = GetComponent<EnemyAIController>();
        }

        private void OnEnable()
        {
            respawnEnemyChannel.onEventRaised += Respawn;
        }

        private void OnDisable()
        {
            respawnEnemyChannel.onEventRaised -= Respawn;
        }

        private void Start()
        {
            m_respawnLocation = transform.position;
            m_respawnLookingDirection = m_enemyAIController.LookingDirection;
        }

        [Button]
        private void Respawn()
        {
            transform.position = m_respawnLocation;
            m_enemyAIController.ChangeLookingDirection(m_respawnLookingDirection);
            onRespawn?.Invoke();
        }
    }
}