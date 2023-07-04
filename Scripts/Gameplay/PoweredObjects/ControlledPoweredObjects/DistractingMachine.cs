using System;
using Characters.Controls.Controllers.AIControllers.Enemies.Units.ControllerImplementation;
using Gameplay.InteractionSystem.Interactables;
using GeneralScriptableObjects;
using SavingSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.PoweredObjects.ControlledPoweredObjects
{
    public class DistractingMachine : MonoBehaviour, IGuidSavable
    {
        public bool MachineOn { get; private set; }
        public bool TurningOff { get; private set;}
        
        public UnityEvent onTurnMachineOn;
        public UnityEvent onTurnMachineOff;
        public UnityEvent onMachineTurnedOff;
        
        public StringVariable TurnOnAction;
        public StringVariable TurnOffAction;
        
        public StringVariable AvailableAction { get; private set; }

        public PolygonCollider2D DetectionCollider { get; private set; }

        [SerializeField] private SentryAIController[] sentryToNotifyOnMachineOn;
        
        public DistractingMachineInteractable Interactable { get; private set; }

        [SerializeField] private Transform interactionLocation;
        public Transform InteractionLocation => interactionLocation;
        
        public string SaveKey { get; private set; }
        public string Filepath { get; private set; }
        
        public ObjectUniqueIdentifier Guid { get; private set; }

        private void Awake()
        {
            DetectionCollider = GetComponent<PolygonCollider2D>();
            Interactable = GetComponentInChildren<DistractingMachineInteractable>();
            
            TurnMachineOn();
        }

        private void Start()
        {
            UpdateAvailableAction();
        }

        private void UpdateAvailableAction()
        {
            AvailableAction = MachineOn? TurnOffAction : TurnOnAction;
        }

        public void TurnMachineOn()
        {
            MachineOn = true;
            TurningOff = false;
            onTurnMachineOn?.Invoke();
            
            foreach (var sentry in sentryToNotifyOnMachineOn)
            {
                sentry.MonitoredMachineTurnedOn(this);
            }
            
            UpdateAvailableAction();
        }

        public void TurnMachineOff()
        {
            onTurnMachineOff?.Invoke();
            TurningOff = true;
        }
        
        //Called by animation event
        public void MachineTurnedOff()
        {
            MachineOn = false;
            TurningOff = false;
            UpdateAvailableAction();
            onMachineTurnedOff?.Invoke();
            
            foreach (var sentry in sentryToNotifyOnMachineOn)
            {
                sentry.MonitoredMachineTurnedOff(this);
            }
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
            ES3.Save(SaveKey + "_turnedOn", true, Filepath);
        }

        public void Load()
        {
            if(SaveKey == null) GetSaveInfo();
            if (!ES3.KeyExists(SaveKey + "_turnedOn", Filepath)) return;
            
            MachineOn = ES3.Load(SaveKey + "_turnedOn", Filepath, false);
        }

        public void Initialize()
        {
            if (MachineOn)
            {
                TurnMachineOn();
            }

            else
            {
                MachineTurnedOff();
            }
        }
    }
}