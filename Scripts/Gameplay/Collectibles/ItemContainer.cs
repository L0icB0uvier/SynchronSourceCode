using System;
using GeneralScriptableObjects.Events;
using SavingSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Collectibles
{
    public class ItemContainer : MonoBehaviour, IGuidSavable
    {
        [SerializeField] private PickableItem containerObjectType;
        public PickableItem ContainerObjectType => containerObjectType;
        
        [SerializeField] private bool containItem;
        public bool ContainItem => containItem;

        public UnityEvent onItemStored;
        public UnityEvent onItemPickedUp;

        public UnityAction onContainerChanged;

        [SerializeField] private ItemContainerInteractedEventChannelSO itemPickedUpEventChannel;
        [SerializeField] private ItemContainerInteractedEventChannelSO itemStoredEventChannel;
        
        public ObjectUniqueIdentifier Guid { get; private set; }
        public string SaveKey { get; private set; }
        public string Filepath { get; private set; }

        private void Awake()
        {
            GetSaveInfo();
        }

        private void ContainItemChanged()
        {
            if (ContainItem)
            {
                onItemStored?.Invoke();
            }

            else
            {
                onItemPickedUp?.Invoke();
            }
            
            onContainerChanged?.Invoke();
        }

        public void TakeItem()
        {
            containItem = false;
            itemPickedUpEventChannel.RaiseEvent(this);
            onItemPickedUp?.Invoke();
            onContainerChanged?.Invoke();
        }

        public void StoreItem()
        {
            containItem = true;
            itemStoredEventChannel.RaiseEvent(this);
            onContainerChanged?.Invoke();
        }

        public void ItemStored()
        {
            onItemStored?.Invoke();
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
            ES3.Save(SaveKey + "_containItem", containItem, Filepath);
        }

        public void Load()
        {
            if(SaveKey == null) GetSaveInfo();
            if (!ES3.KeyExists(SaveKey + "_containItem", Filepath)) return;
            
            containItem = ES3.Load(SaveKey + "_containItem",Filepath,false);
        }

        public void Initialize()
        {
            ContainItemChanged();
        }
    }
}