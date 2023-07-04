using System;
using GeneralScriptableObjects.Events;
using PixelCrushers;
using UnityEngine;
using UnityEngine.Events;

namespace SavingSystem
{
    public class SceneDataInitializer : MonoBehaviour
    {
        [SerializeField] private VoidEventChannelSO saveEventChannel;
        [SerializeField] private VoidEventChannelSO loadEventChannel;
        [SerializeField] private VoidEventChannelSO initializeEventChannel;

        [SerializeField] private UnityEvent onSceneStart;
        private SavedGameData m_savedGameData;
        
        private void Start()
        {
            ManageSceneData();
            onSceneStart?.Invoke();
        }

        private void ManageSceneData()
        {
            if (ES3.FileExists("savedGame/" + gameObject.scene.name + "_SavedData.es3"))
            {
                loadEventChannel.RaiseEvent();
            }

            else
            {
                saveEventChannel.RaiseEvent();
                initializeEventChannel.RaiseEvent();
            }
        }
    }
}