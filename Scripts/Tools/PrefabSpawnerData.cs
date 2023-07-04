using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Create Prefab Spawner Data", order = 1)]
    public class PrefabSpawnerData : ScriptableObject
    {
        public PrefabTypeInfo[] prefabType = new PrefabTypeInfo[0];
    }

    [Serializable]
    public class PrefabTypeInfo
    {
        public EPrefabCategory prefabCategory;
        public PrefabCategoryInfo[] prefabCategoryInfo = new PrefabCategoryInfo[0];
    }

    [Serializable]
    public struct PrefabCategoryInfo
    {
        public string categoryName;
        public bool displayIcon;
        public PrefabInfo[] prefabInfo;
    }

    [Serializable]
    public struct PrefabInfo
    {
        public GameObject prefab;
        public Texture prefabImage;
        public string prefabName;
        public bool hasParent;
        [ShowIf("hasParent")]
        public string parentName;
        public bool spawnAsPrefab;
        public bool spawnAtScreenCenter;
        [HideIf("spawnAtScreenCenter")]
        public Vector3 spawnPosition;
    }

    public enum EPrefabCategory
    {
        Gameplay,
        Navigation,
        Camera,
        Dialog,
        Visual
    }
}