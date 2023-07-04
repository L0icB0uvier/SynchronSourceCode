using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using Gameplay.InteractionSystem.Interactables;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using SavingSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Gameplay.PoweredObjects.ControlledPoweredObjects
{
    public abstract class MovingPoweredSystem : PoweredSystem, IGuidSavable
    {
        public List<MovingPoweredSystemInteractable> ControlledSystemInteractables { get; private set; }
        
        [SerializeField][FoldoutGroup("References")]
        protected PolygonCollider2D aIDetectionCollider;

        public PolygonCollider2D AIDetectionCollider => aIDetectionCollider;

        [SerializeField][FoldoutGroup("References")]
        protected Collider2D[] elementColliders = Array.Empty<Collider2D>();

        [FoldoutGroup("Settings")][PropertyOrder(0)]
        [SerializeField] private EControlledElementState controlledElementState = EControlledElementState.DefaultState;

        public EControlledElementState ControlledElementState => controlledElementState;

        [FoldoutGroup("Settings/AISettings")][PropertyOrder(0)]
        public bool isMonitored;

        [FoldoutGroup("Settings/AISettings")][EnumPaging][ShowIf("isMonitored")][PropertyOrder(0)]
        public EControlledElementState monitoredState = EControlledElementState.DefaultState;
    
        [FoldoutGroup("Settings")][PropertyOrder(0)]
        public bool returnToDefaultOnPowerLost;
        
        public bool IsTransitioning { get; private set; }
        
        [FoldoutGroup("Events")][PropertyOrder(4)]
        public UnityAction onTransitionOver;
        
        [FoldoutGroup("Events")][PropertyOrder(5)]
        public UnityEvent onPlayDefault;
        
        [FoldoutGroup("Events")][PropertyOrder(5)]
        public UnityEvent onPlayAltered;
        
        [FoldoutGroup("Events")][PropertyOrder(5)]
        public UnityEvent onTransitionToAltered;
    
        [FoldoutGroup("Events")][PropertyOrder(5)]
        public UnityEvent onTransitionToDefault; 
        
        [FoldoutGroup("Events")][PropertyOrder(5)]
        public UnityEvent onAlteredStateReached;
    
        [FoldoutGroup("Events")][PropertyOrder(5)]
        public UnityEvent onDefaultStateReached;

        [FoldoutGroup("Events")][PropertyOrder(5)]
        public UnityEvent onColliderUpdated;

        [FoldoutGroup("Events")][PropertyOrder(5)]
        public UnityEvent onInitialise;
        
        [FoldoutGroup("Events")][PropertyOrder(5)]
        public UnityEvent onInteracterEnter; 
        
        [FoldoutGroup("Events")][PropertyOrder(5)]
        public UnityEvent onInteracterExit;

        public VoidEventChannelSO onPoweredSystemColliderChanged;
        
        [SerializeField][FoldoutGroup("Settings/Constant")][PropertyOrder(1)]
        protected StringVariable[] availableActions = new StringVariable[2];

        public StringVariable AvailableAction { get; private set; }
        
        public string SaveKey { get; private set; }
        public string Filepath { get; private set; }
        public ObjectUniqueIdentifier Guid { get; private set; }
        
        private void Awake()
        {
            GetSaveInfo();
            ControlledSystemInteractables = transform.GetComponentsInChildren<MovingPoweredSystemInteractable>().ToList();
        }
        
        protected abstract void UpdateSystemColliders();
        
        protected void ColliderChanged()
        {
            onColliderUpdated?.Invoke();
            onPoweredSystemColliderChanged.RaiseEvent();
        }
        
        public void SwitchSystemState()
        {
            switch (ControlledElementState)
            {
                case EControlledElementState.DefaultState:
                    MoveToAlteredState();
                    break;
            
                case EControlledElementState.AlteredState:
                    MoveToDefaultState();
                    break;
            }
        }
        
        protected override void OnLosePower()
        {
            base.OnLosePower();
            if(returnToDefaultOnPowerLost) MoveToDefaultState();
        }

        public void OnInteracterEnter()
        {
            onInteracterEnter?.Invoke();
        } 
        
        public void OnInteracterExit()
        {
            onInteracterExit?.Invoke();
        }
        
        private void MoveToAlteredState()
        {
            if (!IsTransitioning && ControlledElementState == EControlledElementState.AlteredState) return;
            onTransitionToAltered?.Invoke();

            IsTransitioning = true;
        }

        private void MoveToDefaultState()
        {
            if (!IsTransitioning && ControlledElementState == EControlledElementState.DefaultState) return;
            onTransitionToDefault?.Invoke();

            IsTransitioning = true;
        }

        //Called by animation event
        public void DefaultStateReached()
        {
            controlledElementState = EControlledElementState.DefaultState;
            IsTransitioning = false;
            AvailableAction = availableActions[0];
            onTransitionOver?.Invoke();
            onDefaultStateReached?.Invoke();
        }

        //Called by animation event
        public void AlteredStateReached()
        {
            controlledElementState = EControlledElementState.AlteredState;
            IsTransitioning = false;
            AvailableAction = availableActions[1];
            onTransitionOver?.Invoke();
            onAlteredStateReached?.Invoke();
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
            ES3.Save(SaveKey + "_state", ControlledElementState, Filepath);
        }

        public void Load()
        {
            if(SaveKey == null) GetSaveInfo();
            if (!ES3.KeyExists(SaveKey + "_state", Filepath)) return;
            
            controlledElementState = ES3.Load(SaveKey + "_state", Filepath, EControlledElementState.DefaultState);
        }
        
        public void Initialize()
        {
            onInitialise?.Invoke();

            switch (Powered)
            {
                case true:
                    OnGainPower();
                    switch (ControlledElementState)
                    {
                        case EControlledElementState.DefaultState:
                            onPlayDefault?.Invoke();
                            AvailableAction = availableActions[0];
                            break;
                        case EControlledElementState.AlteredState:
                            onPlayAltered?.Invoke();
                            AvailableAction = availableActions[1];
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case false:
                    OnLosePower();
                    controlledElementState = EControlledElementState.DefaultState;
                    onPlayDefault?.Invoke();
                    AvailableAction = availableActions[0];
                    break;
            }
            
            UpdateSystemColliders();
        }
    }

    [Serializable]
    public class SharedControlledElement : SharedVariable<MovingPoweredSystem>
    {
        public static implicit operator SharedControlledElement(MovingPoweredSystem value)
        { return new SharedControlledElement { Value = value }; }
    }
    
    public enum EControlledElementState
    {
        DefaultState = 1 << 0, 
        AlteredState = 1 << 1,
    }
}