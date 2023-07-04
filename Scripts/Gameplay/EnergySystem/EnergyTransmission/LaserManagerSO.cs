using System;
using System.Collections.Generic;
using System.Linq;
using Lean.Pool;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Gameplay.EnergySystem.EnergyTransmission
{
    [CreateAssetMenu(menuName = "Gameplay/Laser manager")]
    public class LaserManagerSO : ScriptableObject
    {
        private readonly Queue<SpawnLaserRequest> m_spawnLaserRequests = new Queue<SpawnLaserRequest>();
        [SerializeField] private AssetReference _energyLaserAssetRef;

        private bool m_treatingRequest;
        
        public List<LaserChain> m_laserChains = new List<LaserChain>();

        private UnityAction m_spawnLaserCallback;

        private void OnEnable()
        {
            m_treatingRequest = false;
            
            m_laserChains.Clear();
        }

        public void RequestEnergyLaser(ITransmitLaser transmitter, IReceiveLaser receiver, UnityAction onLaserSpawned)
        {
            m_spawnLaserCallback = onLaserSpawned;
            m_spawnLaserRequests.Enqueue(new SpawnLaserRequest(transmitter, receiver));
            if(!m_treatingRequest) LoadAssetReference();
        } 
        
        public void RequestEnergyLaser(ITransmitLaser transmitter, IReceiveLaser receiver)
        {
            m_spawnLaserRequests.Enqueue(new SpawnLaserRequest(transmitter, receiver));
            if(!m_treatingRequest) LoadAssetReference();
        }

        private void LoadAssetReference()
        {
            m_treatingRequest = true;
            _energyLaserAssetRef.LoadAssetAsync<GameObject>().Completed += TreatRequests;
        }
        
        private void SpawnEnergyLaser(GameObject laserPrefab, SpawnLaserRequest request)
        {
            var energyLaser = LeanPool.Spawn(laserPrefab).GetComponent<EnergyLaser>();
            energyLaser.Initialise(request._transmitter, request._receiver);
            request._transmitter.OutgoingLaserConnected(energyLaser);
            request._receiver.IncomingLaserConnected(energyLaser);
            m_spawnLaserCallback?.Invoke();
        }

        private void TreatRequests(AsyncOperationHandle<GameObject> handle)
        {
            for (int i = m_spawnLaserRequests.Count - 1; i >= 0; i--)
            {
                SpawnEnergyLaser(handle.Result, m_spawnLaserRequests.Dequeue());
            }

            Addressables.Release(handle);
            m_treatingRequest = false;
        }

        public void CreateNewLaserChain(Transform transmitter)
        {
            m_laserChains.Add(new LaserChain(transmitter));
        }

        public void DestroyChain(Transform chainSource)
        {
            var laserChain = FindTransmitterChain(chainSource);
            if(laserChain != null)
            {
                m_laserChains.Remove(laserChain);
            }
        }

        public void AddTransmitterToChain(Transform previousTransmitter, Transform transmitterToAdd)
        {
            var laserChain = FindTransmitterChain(previousTransmitter);
            if (laserChain != null)
            {
                laserChain.AddTransmitterToChain(transmitterToAdd);
            }
        }

        public void RemoveTransmitterFromChain(Transform transmitter)
        {
            var laserChain = FindTransmitterChain(transmitter);
            if (laserChain != null)
            {
                FindTransmitterChain(transmitter).RemoveTransmitterFromChain(transmitter);
            }
        }

        private LaserChain FindTransmitterChain(Transform transmitter)
        {
            if (m_laserChains.Count == 0 || !m_laserChains.Any(x => x.ContainTransmitter(transmitter))) return null;
            
            return m_laserChains.First(x => x.ContainTransmitter(transmitter));
        }

        public bool IsTransmitterOnCurrentChain(Transform transmitterOnChain, Transform currentTransmitter)
        {
            var laserChain = FindTransmitterChain(transmitterOnChain);
            return laserChain != null && laserChain.ContainTransmitter(currentTransmitter);
        }
        
        private class SpawnLaserRequest
        {
            public ITransmitLaser _transmitter;
            public IReceiveLaser _receiver;

            public SpawnLaserRequest(ITransmitLaser transmitter, IReceiveLaser receiver)
            {
                _transmitter = transmitter;
                _receiver = receiver;
            }
        }

        [Serializable]
        public class LaserChain
        {
            public List<Transform> m_transmitterChain = new List<Transform>();

            public LaserChain(Transform initialTransmitter)
            {
                m_transmitterChain.Add(initialTransmitter);
            }

            public void RemoveTransmitterFromChain(Transform transmitter)
            {
                if (!m_transmitterChain.Contains(transmitter)) return;
                m_transmitterChain.Remove(transmitter);
            }

            public void AddTransmitterToChain(Transform transmitter)
            {
                if(m_transmitterChain.Contains(transmitter)) return;
                m_transmitterChain.Add(transmitter);
            }

            public bool ContainTransmitter(Transform transmitter)
            {
                return m_transmitterChain.Contains(transmitter);
            }
        }
    }
}