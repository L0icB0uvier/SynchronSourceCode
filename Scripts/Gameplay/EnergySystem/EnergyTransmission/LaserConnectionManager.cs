using Characters.Controls.Controllers.PlayerControllers;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using Lean.Pool;
using UI;
using UI.CallToActionUI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Gameplay.EnergySystem.EnergyTransmission
{
    [CreateAssetMenu(menuName = "Gameplay/Connect Laser Manager")]
    public class LaserConnectionManager : ScriptableObject
    {
        private ConnectLaserUI m_connectLaserUI;
        
        [SerializeField] private AssetReference _connectLaserUIAssetRef;
        [SerializeField] private LaserManagerSO _laserManagerSo;
        
        [Header("Broadcasting on")]
        [SerializeField] private InteractionUIEventChannelSO _addUIActionEventChannelSo;
        [SerializeField] private InteractionUIEventChannelSO _changeUIActionEventChannelSo;
        [SerializeField] private InteractionUIEventChannelSO _removeUIActionEventChannelSo;
        [SerializeField] private ClearActionUIEventChannelSO _clearUIActionEventChannelSo;
        
        [SerializeField][Space] private ConnectLaserEventChannelSO _connectLaserStartEvent;
        [SerializeField] private VoidEventChannelSO _connectLaserEndEvent;

        [SerializeField] private VoidEventChannelSO[] cancelConnectionChannels;
        [SerializeField] private TransmitterPowerLostChannel transmitterPowerLostChannel;
        
        [Header("UIActions")] 
        [SerializeField] private StringVariable _cancelLaserConnectionAction;

        [SerializeField] private ActionUISettings _actionUISettings;
        
        private ITransmitLaser m_transmitter;
        private IReceiveLaser m_receiver;

        private Transform m_connectingTransform;

        private UnityAction m_laserConnectedCallback;

        public bool IsConnectingLaser { get; private set; }
        
        private void OnEnable()
        {
            m_transmitter = null;
            m_receiver = null;
            m_connectingTransform = null;
            IsConnectingLaser = false;

            transmitterPowerLostChannel.OnEventRaised += OnTransmitterPowerLost;
            
            foreach (var cancelChannel in cancelConnectionChannels)
            {
                cancelChannel.onEventRaised += StopConnection;
            }
        }

        private void OnDisable()
        {
            transmitterPowerLostChannel.OnEventRaised -= OnTransmitterPowerLost;
            
            foreach (var cancelChannel in cancelConnectionChannels)
            {
                cancelChannel.onEventRaised -= StopConnection;
            }
        }

        private void OnTransmitterPowerLost(ITransmitLaser transmitter)
        {
            if (m_transmitter == transmitter)
            {
                StopConnection();
            }
        }

        public void StartConnectingLaser(ITransmitLaser transmitter, Transform connectingTransform)
        {
            if (m_connectLaserUI != null) return;
            IsConnectingLaser = true;
            m_transmitter = transmitter;
            m_connectingTransform = connectingTransform;
            SpawnConnectLaserUI();
        }
        
        private void SpawnConnectLaserUI()
        {
            _connectLaserUIAssetRef.LoadAssetAsync<GameObject>().Completed += ConnectLaserPrefabLoaded;
        }

        private void ConnectLaserPrefabLoaded(AsyncOperationHandle<GameObject> handle)
        {
            m_connectLaserUI = LeanPool.Spawn(handle.Result).GetComponent<ConnectLaserUI>();
                
            Addressables.Release(handle);
                
            m_connectLaserUI.Initialise(m_transmitter, m_connectingTransform);
            _connectLaserStartEvent.RaiseEvent(m_connectLaserUI);
            _addUIActionEventChannelSo.RaiseEvent(EPlayerCharacterType.Skullface, _cancelLaserConnectionAction, _actionUISettings);
        }

        public void ChangeConnectLaserUIOrigin(ITransmitLaser transmitter)
        {
            m_transmitter = transmitter;
            m_connectLaserUI.ChangeOrigin(transmitter);
            _connectLaserStartEvent.RaiseEvent(m_connectLaserUI);
            _addUIActionEventChannelSo.RaiseEvent(EPlayerCharacterType.Skullface, _cancelLaserConnectionAction, _actionUISettings);
        }

        public void EnterConnectInteractable(Transform interactableTransform)
        {
            if (m_connectLaserUI == null) return;
            m_connectLaserUI.ChangeEndTarget(interactableTransform, true);
        }

        public void ExitConnectInteractable()
        {
            if (m_connectLaserUI == null) return;
            m_connectLaserUI.ChangeEndTarget(m_connectingTransform, false);
        }

        public void StopConnection()
        {
            if (IsConnectingLaser == false) return;
            
            DespawnConnectLaserUI();
            IsConnectingLaser = false;
            _clearUIActionEventChannelSo.RaiseEvent(EPlayerCharacterType.Skullface);
            _connectLaserEndEvent.RaiseEvent();
        }

        private void OnLaserSpawned()
        {
            m_laserConnectedCallback?.Invoke();
        }

        private void DespawnConnectLaserUI()
        {
            if (m_connectLaserUI == null) return;
            LeanPool.Despawn(m_connectLaserUI.gameObject);
            m_connectLaserUI = null;
        }
        
        public void ConnectLaser(IReceiveLaser receiver, UnityAction laserConnectedCallback)
        {
            if (receiver.ReceiverTransform.root == m_transmitter.TransmitterTransform.root) return;
            m_laserConnectedCallback = laserConnectedCallback;
            m_receiver = receiver;
            _laserManagerSo.RequestEnergyLaser(m_transmitter, m_receiver, OnLaserSpawned);
        } 
        
        public void ConnectLaser(IReceiveLaser receiver)
        {
            if (receiver.ReceiverTransform.root == m_transmitter.TransmitterTransform.root) return;
            m_laserConnectedCallback = null;
            m_receiver = receiver;
            _laserManagerSo.RequestEnergyLaser(m_transmitter, m_receiver);
        }
    }
}