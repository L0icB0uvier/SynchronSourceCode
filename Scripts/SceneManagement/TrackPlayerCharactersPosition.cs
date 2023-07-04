using System;
using GeneralScriptableObjects.Events;
using UnityEngine;

namespace SceneManagement
{
    public class TrackPlayerCharactersPosition : MonoBehaviour
    {
        [Header("Listening to")]
        [SerializeField] private VoidEventChannelSO _onCharacterSpawned;
        [SerializeField] private VoidEventChannelSO _onChangeScene;

        private bool m_trackingPlayerCharacterPos;
        [SerializeField] private Transform[] m_transforms = new Transform[2];

        [SerializeField] private bool moveByRigidBody;
        
        private Rigidbody2D m_rbd2;
        
        private void Awake()
        {
            if(moveByRigidBody) m_rbd2 = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _onCharacterSpawned.onEventRaised += StartTrackingPosition;
            _onChangeScene.onEventRaised += StopTrackingPosition;
        }
        
        private void StartTrackingPosition()
        {
            if (!GameObject.FindWithTag("Hicks") || !GameObject.FindWithTag("Skullface")) return;

            m_transforms[0] = GameObject.FindWithTag("Hicks").transform;
            m_transforms[1] = GameObject.FindWithTag("Skullface").transform;
            
            m_trackingPlayerCharacterPos = true;
            _onCharacterSpawned.onEventRaised -= StartTrackingPosition;
        }

        private void StopTrackingPosition()
        {
            m_trackingPlayerCharacterPos = false;
            m_transforms[0] = null;
            m_transforms[1] = null;
        }
        
        private void FixedUpdate()
        {
            if (!m_trackingPlayerCharacterPos || !m_transforms[0] || !m_transforms[1]) return;
            
            if (moveByRigidBody)
            {
                m_rbd2.MovePosition((m_transforms[0].position + m_transforms[1].position) / 2);
            }
            else
            {
                transform.position = (m_transforms[0].position + m_transforms[1].position) / 2;
            }
        }

        private void OnDestroy()
        {
            _onChangeScene.onEventRaised -= StopTrackingPosition;
        }
    }
}