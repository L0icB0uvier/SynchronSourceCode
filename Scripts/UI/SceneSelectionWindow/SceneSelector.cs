using System;
using GeneralScriptableObjects.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SceneSelectionWindow
{
    public class SceneSelector : MonoBehaviour
    {
        private LocationSO m_sceneLocation;
        private PathSO m_scenePath;

        public Button button { get; private set; }

        [SerializeField] private TMP_Text text;
        [SerializeField] private Image image;

        [SerializeField] private SceneSelectedEventChannel sceneSelectedEventChannel;

        private void Awake()
        {
            button = GetComponentInChildren<Button>();
        }

        public void Initialise(LocationSO location, PathSO path)
        {
            m_sceneLocation = location;
            m_scenePath = path;
            image.sprite = m_sceneLocation.sceneImage;
            text.text = m_sceneLocation.locationName;
        }

        public void OnSceneSelected()
        {
            sceneSelectedEventChannel.RaiseEvent(m_sceneLocation, m_scenePath);
        }
    }
}
