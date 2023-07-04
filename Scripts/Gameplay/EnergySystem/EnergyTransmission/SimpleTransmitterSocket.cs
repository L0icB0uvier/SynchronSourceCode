using System.Linq;
using GeneralScriptableObjects.Events;
using SavingSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.EnergySystem.EnergyTransmission
{
    public class SimpleTransmitterSocket : MonoBehaviour, IReceiveLaser, ITransmitLaser, IGuidSavable
    {
        public Transform TransmitterTransform => transform;
        public Transform ReceiverTransform => transform;
        public bool IsReceivingLaser { get; private set; }

        public EnergyLaser OutgoingEnergyLaser { get; private set; }
        
        [SerializeField] private LaserReceiverIdentifier transmittingTo;
        public LaserReceiverIdentifier TransmittingTo => transmittingTo;
        public UnityAction OnPoweredChanged { get; set; }

        public EnergyLaser IncomingEnergyLaser { get; private set; }
        
        public UnityEvent onReceiveLaser;
        public UnityEvent onLoseLaser;
        
        [SerializeField] private LaserManagerSO _laserManagerSo;

        [SerializeField] private TransmitterPowerLostChannel transmitterPowerLostChannel;

        private void Awake()
        {
            GetSaveInfo();
        }
        
        public void ReceiveLaser()
        {
            if (!IsReceivingLaser)
            {
                IsReceivingLaser = true;
                OnPoweredChanged?.Invoke();
                onReceiveLaser?.Invoke();
                if (OutgoingEnergyLaser != null) OutgoingEnergyLaser.Activate();
            }

            if (OutgoingEnergyLaser != null)
            {
                OutgoingEnergyLaser.UpdateLaser();
            }
        }

        public void LoseLaser()
        {
            IsReceivingLaser = false;
            OnPoweredChanged?.Invoke();
            onLoseLaser?.Invoke();
            transmitterPowerLostChannel.RaiseEvent(this);
            
            if (OutgoingEnergyLaser != null)
            {
                OutgoingEnergyLaser.Deactivate();
            }
        }

        public void IncomingLaserConnected(EnergyLaser energyLaser)
        {
            IncomingEnergyLaser = energyLaser;
            _laserManagerSo.AddTransmitterToChain(energyLaser.TransmittingSocket.TransmitterTransform, transform);
        }

        public void IncomingLaserDisconnected()
        {
            _laserManagerSo.RemoveTransmitterFromChain(transform);
            IncomingEnergyLaser = null;

            if (OutgoingEnergyLaser != null)
            {
                RemoveOutgoingLaser();
            }
        }
        
        public void OutgoingLaserConnected(EnergyLaser energyLaser)
        {
            OutgoingEnergyLaser = energyLaser;
            transmittingTo = energyLaser.m_receivingSocket.ReceiverTransform.GetComponent<LaserReceiverIdentifier>();
            
            if (IsReceivingLaser)
            {
                OutgoingEnergyLaser.Activate();
            }

            else
            {
                OutgoingEnergyLaser.Deactivate();
            }
        }

        public void OutgoingLaserDisconnected()
        {
            OutgoingEnergyLaser = null;
            transmittingTo = null;
        }

        public void TransmitterRemoved()
        {
            RemoveOutgoingLaser();
            RemoveIncomingLaser();
        }

        public void RemoveOutgoingLaser()
        {
            if(OutgoingEnergyLaser) OutgoingEnergyLaser.DespawnLaser();
        }
        
        public void RemoveIncomingLaser()
        {
            if(IncomingEnergyLaser) IncomingEnergyLaser.DespawnLaser();
        }

        public ObjectUniqueIdentifier Guid { get; private set; }
        public string SaveKey { get; private set; }
        public string Filepath { get; private set; }
        public void GetSaveInfo()
        {
            Guid = GetComponent<ObjectUniqueIdentifier>();
            var go = gameObject;
            SaveKey = go.name + "_" + Guid.id;
            Filepath = "savedGame/sceneData/" + go.scene.name + "_SavedData.es3";
        }

        public void Save()
        {
            if(SaveKey == null) GetSaveInfo();
            if (!transmittingTo)
            {
                ES3.Save(SaveKey + "_transmittingTo", "NotConnected", Filepath);
                return;
            }
            ES3.Save(SaveKey + "_transmittingTo", transmittingTo.ObjectGUID.id, Filepath);
        }
        
        public void PrepareLoad()
        {
            if (OutgoingEnergyLaser != null)
            {
                RemoveOutgoingLaser();
            }

            if (IncomingEnergyLaser != null)
            {
                RemoveIncomingLaser();
            }

            IsReceivingLaser = false;
        }
        public void Load()
        {
            if(SaveKey == null) GetSaveInfo();
            if (!ES3.KeyExists(SaveKey + "_transmittingTo", Filepath)) return;

            var transmitterGuid = ES3.Load<string>(SaveKey + "_transmittingTo", Filepath);

            if (transmitterGuid == "NotConnected")
            {
                transmittingTo = null;
            }

            else
            {
                var receiversInScene = FindObjectsOfType<LaserReceiverIdentifier>();
                transmittingTo = receiversInScene.First(x => x.ObjectGUID.id == transmitterGuid);
            }
        }

        public void Initialize()
        {
            if (transmittingTo == null || OutgoingEnergyLaser != null) return;

            var receiver = transmittingTo.gameObject.GetComponent<IReceiveLaser>();
            _laserManagerSo.RequestEnergyLaser(this, receiver);
        }
    }
}