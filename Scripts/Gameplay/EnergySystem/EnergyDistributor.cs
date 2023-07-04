using System;
using System.Linq;
using Gameplay.EnergySystem.EnergyTransmission;
using Gameplay.PoweredObjects;
using GeneralScriptableObjects.Events;
using SavingSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.EnergySystem
{
    public class EnergyDistributor : PoweredSystem, ITransmitLaser, IGuidSavable
    {
        public Transform TransmitterTransform => transform;
        public bool IsReceivingLaser { get; private set; }
        public EnergyLaser OutgoingEnergyLaser { get; private set; }

        [SerializeField] private LaserReceiverIdentifier transmittingTo;

        public LaserReceiverIdentifier TransmittingTo => transmittingTo;
        public UnityAction OnPoweredChanged { get; set; }
        
        [SerializeField] private LaserManagerSO _laserManagerSo;

        public ObjectUniqueIdentifier Guid { get; private set; }
        public string SaveKey { get; private set; }
        public string Filepath { get; private set; }

        [SerializeField] private TransmitterPowerLostChannel transmitterPowerLostChannel;

        private void Awake()
        {
            GetSaveInfo();
        }

        private void FixedUpdate()
        {
            if (IsReceivingLaser && OutgoingEnergyLaser)
            {
                OutgoingEnergyLaser.UpdateLaser();
            }
        }

        protected override void OnGainPower()
        {
            base.OnGainPower();
            ReceiveEnergy();
        }

        protected override void OnLosePower()
        {
            base.OnLosePower();
            LoseEnergy();
        }

        private void LoseEnergy()
        {
            IsReceivingLaser = false;
            OnPoweredChanged?.Invoke();
            transmitterPowerLostChannel.RaiseEvent(this);
            if(OutgoingEnergyLaser) OutgoingEnergyLaser.Deactivate();
        }

        private void ReceiveEnergy()
        {
            IsReceivingLaser = true;
            OnPoweredChanged?.Invoke();
            if(OutgoingEnergyLaser) OutgoingEnergyLaser.Activate();
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
            _laserManagerSo.CreateNewLaserChain(transform);
        }

        //Callback called after removing OutgoingLaser
        public void OutgoingLaserDisconnected()
        {
            OutgoingEnergyLaser = null;
            transmittingTo = null;
            _laserManagerSo.DestroyChain(transform);
        }

        public void RemoveOutgoingLaser()
        {
            if(OutgoingEnergyLaser) OutgoingEnergyLaser.DespawnLaser();
        }
        
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
                
                if (receiversInScene.Length == 0)
                {
                    transmittingTo = null;
                }
                transmittingTo = receiversInScene.First(x => x.ObjectGUID.id == transmitterGuid);
            }
        }

        public void PrepareLoad()
        {
            if (OutgoingEnergyLaser != null)
            {
                RemoveOutgoingLaser();
            }

            IsReceivingLaser = false;
        }
        
        public void Initialize()
        {
            if (transmittingTo == null || OutgoingEnergyLaser != null) return;

            var receiver = transmittingTo.gameObject.GetComponent<IReceiveLaser>();
            _laserManagerSo.RequestEnergyLaser(this, receiver);
        }
    }
}
