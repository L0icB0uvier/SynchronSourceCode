using System;
using Gameplay.Collectibles;
using Gameplay.EnergySystem.EnergyTransmission;
using GeneralScriptableObjects.Events;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Gameplay.InteractionSystem.InteractionManagers
{
    public class LaserInteractionManager : MonoBehaviour
    {
        [SerializeField] private bool hasItemContainer = true;
        
        [SerializeField][ShowIf("hasItemContainer")][Indent()] private ItemContainer ItemContainer;
        
        private enum ELaserSocketType {TransmitAndReceive, OnlyTransmit, OnlyReceive}

        [SerializeField] private ELaserSocketType laserSocketType;
        
        private ITransmitLaser m_transmitter;

        [SerializeField] private ConnectLaserEventChannelSO _onConnectLaserChannel;
        [SerializeField] private VoidEventChannelSO _onConnectLaserStoppedChannel;
        [SerializeField] private VoidEventChannelSO[] resolveStateChannels;

        [ShowIf("CanReceiveLaser")] public UnityEvent onConnectLaserInteractionPossible;
        [ShowIf("CanTransmitLaser")] public UnityEvent onTransmitLaserInteractionPossible;
        public UnityEvent onInteractionImpossible;
        
        private bool m_playerConnectingLaser;

        [SerializeField] private LaserManagerSO laserManager;

        private ConnectLaserUI m_connectLaserUI;

        [SerializeField] private float linecastStartOffset;
        
        [SerializeField] private LayerMask laserObstacleMask;

        private void Awake()
        {
            switch (laserSocketType)
            {
                case ELaserSocketType.TransmitAndReceive:
                    m_transmitter = GetComponent<ITransmitLaser>();
                    break;
                case ELaserSocketType.OnlyTransmit:
                    m_transmitter = GetComponent<ITransmitLaser>();
                    break;
                case ELaserSocketType.OnlyReceive:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Start()
        {
            ResolveState();
        }

        private void OnEnable()
        {
            if(hasItemContainer) ItemContainer.onContainerChanged += ResolveState;
            if(CanTransmitLaser()) m_transmitter.OnPoweredChanged += ResolveState;
            _onConnectLaserChannel.OnEventRaised += OnConnectLaserStart;
            _onConnectLaserStoppedChannel.onEventRaised += OnConnectLaserEnd;

            foreach (var channel in resolveStateChannels)
            {
                channel.onEventRaised += LayoutChanged;
            }
        }

        private void OnDisable()
        {
            if(hasItemContainer) ItemContainer.onContainerChanged -= ResolveState;
            if(CanTransmitLaser()) m_transmitter.OnPoweredChanged -= ResolveState;
            _onConnectLaserChannel.OnEventRaised -= OnConnectLaserStart;
            _onConnectLaserStoppedChannel.onEventRaised -= OnConnectLaserEnd;
            foreach (var channel in resolveStateChannels)
            {
                channel.onEventRaised -= LayoutChanged;
            }
        }
        
        private void OnConnectLaserStart(ConnectLaserUI connectLaserUI)
        {
            m_connectLaserUI = connectLaserUI;
            m_playerConnectingLaser = true;
            ResolveState();
        }

        private bool ReceiverIsOnCurrentChain()
        {
            return laserManager.IsTransmitterOnCurrentChain(m_connectLaserUI.TransmittingLaserObject.TransmitterTransform, transform);
        }

        private void OnConnectLaserEnd()
        {
            m_playerConnectingLaser = false;
            m_connectLaserUI = null;
            ResolveState();
        }

        private void LayoutChanged()
        {
            ResolveState();
        }

        private void ResolveState()
        {
            if (hasItemContainer && !ItemContainer.ContainItem)
            {
                onInteractionImpossible?.Invoke();
                return;
            }
           
            switch (m_playerConnectingLaser)
            {
                case true:
                    if (!CanReceiveLaser() || IsLineOfSightObstructed())
                    {
                        onInteractionImpossible?.Invoke();
                        return;
                    }

                    switch (ReceiverIsOnCurrentChain())
                    {
                        case true:
                            onInteractionImpossible?.Invoke();
                            break;
                        
                        case false:
                            onConnectLaserInteractionPossible?.Invoke();
                            break;
                    }
                    break;
                
                case false:
                    if (!CanTransmitLaser())
                    {
                        onInteractionImpossible?.Invoke();
                        return;
                    }
                    
                    switch (m_transmitter.IsReceivingLaser)
                    {
                        case true:
                            onTransmitLaserInteractionPossible?.Invoke();
                            break;
                        
                        case false:
                            onInteractionImpossible?.Invoke();
                            break;
                    }
                    break;
            }
        }

        private bool CanReceiveLaser()
        {
            return laserSocketType switch
            {
                ELaserSocketType.TransmitAndReceive => true,
                ELaserSocketType.OnlyTransmit => false,
                ELaserSocketType.OnlyReceive => true,
                _ => false
            };
        } 
        
        private bool CanTransmitLaser()
        {
            return laserSocketType switch
            {
                ELaserSocketType.TransmitAndReceive => true,
                ELaserSocketType.OnlyTransmit => true,
                ELaserSocketType.OnlyReceive => false,
                _ => false
            };
        }

        private bool IsLineOfSightObstructed()
        {
            var startPos = transform.position;
            var endPos = m_connectLaserUI.TransmittingLaserObject.TransmitterTransform.position;
            var dir = MathCalculation.GetDirectionalVectorBetween2Points(startPos,
                endPos);

            Vector2 raycastStart = (Vector2)startPos + dir * linecastStartOffset;
            Vector2 raycastEnd = (Vector2)endPos - dir * linecastStartOffset;

            var hit = Physics2D.Linecast(raycastStart, raycastEnd, laserObstacleMask);
            return hit && hit.collider.transform.parent != transform;
        }
    }
}