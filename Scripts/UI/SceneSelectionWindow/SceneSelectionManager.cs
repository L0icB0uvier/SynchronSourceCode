using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace UI.SceneSelectionWindow
{
    public class SceneSelectionManager : MonoBehaviour
    {
        [SerializeField] private SceneInfo[] sceneSelectionInfo;
        private List<SceneSelector> m_sceneSelectors = new List<SceneSelector>();

        [SerializeField] private AssetReference sceneSelectionItemReference;
        [SerializeField] private Button returnButton;
        
        private bool m_gridPopulated;

        void Start() {
            AsyncOperationHandle handle = sceneSelectionItemReference.LoadAssetAsync<GameObject>();
            handle.Completed += Handle_Completed;
        }

        // Instantiate the loaded prefab on complete
        private void Handle_Completed(AsyncOperationHandle obj) {
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var sceneInfo in sceneSelectionInfo)
                {
                    GenerateSceneSelector(sceneInfo);
                }

                m_gridPopulated = true;
                InitialiseWindow();
            } 
        
            else 
            {
                Debug.LogError("AssetReference failed to load.");
            }
        
            Addressables.Release(obj);
        }

        private void OnEnable()
        {
            if (m_gridPopulated)
            {
                InitialiseWindow();
            }
        }

        private void InitialiseWindow()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(transform.GetChild(0).GetChild(0).gameObject);
            
            SetButtonsNavigation();
        }

        private void SetButtonsNavigation()
        {
            var firstButtonNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnDown = m_sceneSelectors[4].button,
                selectOnRight = m_sceneSelectors[1].button,
                selectOnLeft = returnButton,
                selectOnUp = m_sceneSelectors[12].button
            };
            m_sceneSelectors[0].button.navigation = firstButtonNav;

            var lastButtonNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnDown = m_sceneSelectors[0].button,
                selectOnRight = returnButton,
                selectOnLeft = returnButton,
                selectOnUp = m_sceneSelectors[8].button
            };

            m_sceneSelectors[m_sceneSelectors.Count - 1].button.navigation = lastButtonNav;

            var returnNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnDown = m_sceneSelectors[0].button,
                selectOnRight = m_sceneSelectors[0].button,
                selectOnLeft = m_sceneSelectors[12].button,
                selectOnUp = m_sceneSelectors[11].button
            };

            returnButton.navigation = returnNav;
        }

        private void GenerateSceneSelector(SceneInfo sceneInfo)
        {
            var instantiatedObject = (GameObject)Instantiate(sceneSelectionItemReference.Asset, transform);
            var sceneSelector = instantiatedObject.GetComponent<SceneSelector>();
            sceneSelector.Initialise(sceneInfo.location, sceneInfo.path);
            m_sceneSelectors.Add(sceneSelector);
        }
    }

    [Serializable]
    public class SceneInfo
    {
        public LocationSO location;
        public PathSO path;
    }
}