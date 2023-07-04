using System;
using System.Collections.Generic;
using System.Linq;
using SavingSystem;
using SceneManagement.LevelManagement;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Tools.Editor
{
    public class PrefabSpawner : EditorWindow
    {
        private static PrefabSpawnerData m_Data;

        private Vector2 m_scrollPosition;

        private float m_iconSize = 128;

        private GUIStyle m_sliderStyle;

        private GUIStyle m_thumbStyle;

        private GUIStyle m_imageButtonStyle;
        
        private GUIStyle m_textButtonStyle;

        private GUIStyle m_categoryLabelStyle;

        private GUIStyle m_categoryStyle;

        private GUIStyle m_categoryTypeStyle;

        private GUIStyle m_categoryTypeLabelStyle;

        [MenuItem("Tools/Prefab Spawner")]
        public static void ShowWindow()
        {
            PrefabSpawner window = (PrefabSpawner)GetWindow(typeof(PrefabSpawner), false,"Prefab Spawner");
            m_Data = GetData();
            window.Show();
        }

        private static PrefabSpawnerData GetData()
        {
            var loadedData = (PrefabSpawnerData)AssetDatabase.LoadAssetAtPath("Assets/Scripts/Tools/Data/PrefabSpawnerData.asset",
                typeof(PrefabSpawnerData));
            if (!loadedData)
            {
                Debug.LogError("There is no data file at the specifiedLocation");
                return null;
            }

            return loadedData;
        }

        private void OnGUI()
        {
            if (!m_Data)
            {
                m_Data = GetData();
            }

            DrawIconSizeSlider();
            DrawCategories();
        }

        private void DrawCategories()
        {
            SetStyles();

            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition);

            foreach (var prefabType in m_Data.prefabType)
            {
                GUILayout.BeginVertical(m_categoryStyle);

                GUILayout.Label(prefabType.prefabCategory.ToString(), m_categoryLabelStyle);
                GenerateCategory(prefabType.prefabCategoryInfo, m_imageButtonStyle);
                GUILayout.EndVertical();
            }


            GUILayout.EndScrollView();
        }

        private void DrawIconSizeSlider()
        {
            GUIStyle buttonSizeSlideAreaStyle = new GUIStyle();
            buttonSizeSlideAreaStyle.alignment = TextAnchor.MiddleRight;

            GUILayout.BeginHorizontal(buttonSizeSlideAreaStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label("Button size");
            SetSliderStyles();
            m_iconSize = GUILayout.HorizontalSlider(m_iconSize, 64, 128, m_sliderStyle, m_thumbStyle);
            GUILayout.EndHorizontal();
        }
        

        private Texture2D MakeTex( int width, int height, Color col )
        {
            Color[] pix = new Color[width * height];
            for( int i = 0; i < pix.Length; ++i )
            {
                pix[ i ] = col;
            }
            Texture2D result = new Texture2D( width, height );
            result.SetPixels( pix );
            result.Apply();
            return result;
        }

        private void GenerateCategory(PrefabCategoryInfo[] categoryInfo, GUIStyle buttonStyle)
        {
            foreach (var prefabCategoryInfo in categoryInfo)
            {
                SetCategoryTypeStyle();

                GUILayout.BeginVertical(m_categoryTypeStyle);

                SetCategoryTypeLabelStyle();

                GUILayout.Label(prefabCategoryInfo.categoryName, m_categoryTypeLabelStyle);
                GUILayout.BeginHorizontal();
                
                int selectedButton = -1;
                int xcount = Mathf.FloorToInt((Screen.width - 130) / m_iconSize);

                switch (prefabCategoryInfo.displayIcon)
                {
                    case true:
                        Texture[] buttonTexture = new Texture[prefabCategoryInfo.prefabInfo.Length];
                        
                        for (int i = 0; i < prefabCategoryInfo.prefabInfo.Length; i++)
                        {
                            buttonTexture[i] = prefabCategoryInfo.prefabInfo[i].prefabImage;
                        }
                        
                        selectedButton = GUILayout.SelectionGrid(selectedButton, buttonTexture, xcount, m_imageButtonStyle);
                
                        if (selectedButton >= 0)
                        {
                            CreateAndSetupPrefab(prefabCategoryInfo.prefabInfo[selectedButton]);
                        }
                        
                        break;
                    case false:
                        string[] buttonText = new string[prefabCategoryInfo.prefabInfo.Length];
                        for (int i = 0; i < prefabCategoryInfo.prefabInfo.Length; i++)
                        {
                            buttonText[i] = prefabCategoryInfo.prefabInfo[i].prefabName;
                        }
                        
                        selectedButton = GUILayout.SelectionGrid(selectedButton, buttonText, xcount, m_textButtonStyle);
                
                        if (selectedButton >= 0)
                        {
                            CreateAndSetupPrefab(prefabCategoryInfo.prefabInfo[selectedButton]);
                        }
                        break;
                }
                
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
        }
        
        private void CreateAndSetupPrefab(PrefabInfo prefabInfo)
        {
            var spawnedPrefab = SpawnPrefab(prefabInfo);
            SetName(spawnedPrefab, prefabInfo.prefab);
            GenerateUniqueID(spawnedPrefab);
            if(prefabInfo.hasParent) SetParent(spawnedPrefab.transform, prefabInfo.parentName);
        }

        private static void GenerateUniqueID(GameObject spawnedPrefab)
        {
            var identifier = spawnedPrefab.GetComponent<ObjectUniqueIdentifier>();
            if (identifier != null)
            {
                identifier.GenerateID();
            }
        }

        private GameObject SpawnPrefab(PrefabInfo prefabInfo)
        {
            Vector3 spawnPos;
            if (prefabInfo.spawnAtScreenCenter)
            {
                spawnPos = SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1.0f));
                spawnPos.z = 0;
            }

            else
            {
                spawnPos = prefabInfo.spawnPosition;
            }

            GameObject spawnedPrefab;
            if (prefabInfo.spawnAsPrefab)
            {
                spawnedPrefab = PrefabUtility.InstantiatePrefab(prefabInfo.prefab) as GameObject;
            }

            else
            {
                spawnedPrefab = Instantiate(prefabInfo.prefab);
            }
           
            Undo.RegisterCreatedObjectUndo(spawnedPrefab, "Spawn" + spawnedPrefab.name);

            spawnedPrefab.transform.position = spawnPos;
            Selection.activeGameObject = spawnedPrefab;
            return spawnedPrefab;
        }
        
        private void SetParent(Transform child, string parentName)
        {
            GameObject parent = GameObject.Find(parentName);
            if (!parent)
                parent = new GameObject(parentName);
                            
            child.parent = parent.transform;
        }

        private void SetName(GameObject spawnPrefab, GameObject sourcePrefab)
        {
            var objectsOfType = FindAllInstancesOfPrefab(sourcePrefab);

            string objectName = "";
            bool foundName = false;
            int index = 0;
                                
            while (!foundName)
            {
                objectName = sourcePrefab.name + "_" + index;

                if (objectsOfType.Any(item => item.name == objectName))
                {
                    index++;
                }

                else
                {
                    foundName = true;
                }
            }

            spawnPrefab.name = objectName;
        }
    
        public GameObject[] FindAllInstancesOfPrefab(GameObject prefab)
        {
            var gameObjects = FindObjectsOfType<GameObject>();
            List<GameObject> objectsOfType = new List<GameObject>();
            foreach (var go in gameObjects)
            {
                var goPrefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
                if (goPrefab == prefab)
                {
                    objectsOfType.Add(go);
                }
            }

            return objectsOfType.ToArray();
        }
        
        private void SetSliderStyles()
        {
            m_sliderStyle = new GUIStyle(GUI.skin.horizontalSlider)
            {
                fixedWidth = 128
            };

            m_thumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);
        }

        private void SetStyles()
        {
            m_imageButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = m_iconSize,
                fixedWidth = m_iconSize,
                wordWrap = true,
                fontSize = 15
            };
            
            m_textButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = m_iconSize / 2,
                fixedWidth = m_iconSize,
                wordWrap = true,
                fontSize = 15
            };

            m_categoryLabelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(0,0,10,10),
            };

            m_categoryStyle = new GUIStyle()
            {
                normal =
                {
                    background = MakeTex(2, 2, new Color(.5f, .5f, .5f, .5f))
                },
                margin = new RectOffset(20, 40, 10, 10),
                fixedWidth = Screen.width - 60
            };
        }
        
        private void SetCategoryTypeLabelStyle()
        {
            m_categoryTypeLabelStyle = new GUIStyle(GUI.skin.label);
            m_categoryTypeLabelStyle.fontSize = 15;
            m_categoryTypeLabelStyle.fontStyle = FontStyle.BoldAndItalic;
            m_categoryTypeLabelStyle.alignment = TextAnchor.MiddleLeft;
        }

        private void SetCategoryTypeStyle()
        {
            m_categoryTypeStyle = new GUIStyle(GUI.skin.window)
            {
                margin = new RectOffset(20, 20, 0, 30),
                padding = new RectOffset(10, 10, 10, 10),
                
            };
        }
    }
}
