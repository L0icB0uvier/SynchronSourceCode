using Characters.CharacterAbilities.Inventory;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using UnityEngine;

namespace SavingSystem
{
    [CreateAssetMenu(fileName = "SaveSystem", menuName = "ScriptableObjects/SaveSystem", order = 0)]
    public class SaveSystem : ScriptableObject
    {
        public string locationID;
        public PathStorageSO currentPath;
        public InventorySlots hicksInventory;
        
        [SerializeField] private string GameManagerDataPath = "savedGame/GameManagerData.es3";
        [SerializeField] private string SettingsDataPath = "./Settings.es3";

        [SerializeField] private LocationChangeChannelSO _locationChanged;
        [SerializeField] private VoidEventChannelSO _respawnChannel;
        
        [SerializeField] private VoidEventChannelSO requestSaveChannel;

        [SerializeField] private VoidEventChannelSO initializeInventoryUIChannel;

        public BoolVariable showTutorialAndDialogSetting;
        public FloatVariable masterVolumeSetting;
        public FloatVariable musicVolumeSetting;
        public FloatVariable sfxVolumeSetting;
        
        public bool SavedDataFound
        {
            get => IsPreviousGameDataSaved();
        }
        
        private void OnEnable()
        {
            _locationChanged.OnLoadingRequested += CacheLoadLocations;
            _respawnChannel.onEventRaised += LoadPlayerData;
        }

        private void OnDisable()
        {
            _locationChanged.OnLoadingRequested -= CacheLoadLocations;
            _respawnChannel.onEventRaised -= LoadPlayerData;
        }
        
        public void SaveGameManagerData()
        {
            ES3.Save("locationId", locationID,  GameManagerDataPath);
            ES3.Save("scenePath", currentPath.lastPathTaken, GameManagerDataPath);
            ES3.Save("hicksInventory", hicksInventory, GameManagerDataPath);
        }
        
        public bool LoadSavedDataFromDisk()
        {
            if (!IsPreviousGameDataSaved()) return false;
            
            locationID = ES3.Load<string>("locationId", GameManagerDataPath);
            currentPath.lastPathTaken = ES3.Load<PathSO>("scenePath", GameManagerDataPath);
            LoadPlayerData();

            return true;
        }
        
        private void LoadPlayerData()
        {
            if (ES3.KeyExists("hicksInventory", GameManagerDataPath))
            {
                hicksInventory = ES3.Load<InventorySlots>("hicksInventory", GameManagerDataPath); 
            }

            initializeInventoryUIChannel.RaiseEvent();
        }

        public void InitializePlayerData()
        {
            hicksInventory.EmptyInventory();
        }
        
        private void CacheLoadLocations(LocationSO locationToLoad)
        {
            
            LocationSO locationSO = locationToLoad;
            if (locationSO)
            {
                locationID = locationSO.Guid;
            }
            
            SaveGameManagerData();
            SaveCurrentSceneData();
        }

        public void DeleteCurrentFiles()
        {
            if (ES3.DirectoryExists("savedGame/sceneData"))
            {
                ES3.DeleteDirectory("savedGame/sceneData");
            }

            if (ES3.FileExists(GameManagerDataPath))
            {
                ES3.DeleteFile(GameManagerDataPath);
            }
        }

        private bool IsPreviousGameDataSaved()
        {
            if (!ES3.FileExists(GameManagerDataPath)) return false;
            return ES3.KeyExists("locationId", GameManagerDataPath) &&
                   ES3.KeyExists("scenePath", GameManagerDataPath);
        }
        
        private void SaveCurrentSceneData()
        {
            requestSaveChannel.RaiseEvent();
        }

        public void SaveSettings()
        {
            ES3.Save("ShowDialog", showTutorialAndDialogSetting.Value, SettingsDataPath);
            ES3.Save("masterVolumeSetting", masterVolumeSetting.Value, SettingsDataPath);
            ES3.Save("musicVolumeSetting", musicVolumeSetting.Value, SettingsDataPath);
            ES3.Save("sfxVolumeSetting", sfxVolumeSetting.Value, SettingsDataPath);
        }

        public void LoadSettings()
        {
            if (!ES3.FileExists(SettingsDataPath)) return;

            showTutorialAndDialogSetting.Value = ES3.Load<bool>("ShowDialog", SettingsDataPath);
            masterVolumeSetting.Value = ES3.Load<float>("masterVolumeSetting", SettingsDataPath);
            musicVolumeSetting.Value = ES3.Load<float>("musicVolumeSetting", SettingsDataPath);
            sfxVolumeSetting.Value = ES3.Load<float>("sfxVolumeSetting", SettingsDataPath);
        }
    }
}