using System;
using UnityEngine;

namespace Utilities
{
    [CreateAssetMenu(fileName = "PrefabInstantiationData", menuName = "ScriptableObjects/Create Prefab Instantiation data", order = 2)]
    public class PrefabInstantiationData : ScriptableObject
    {
        public GameObject[] prefabs = Array.Empty<GameObject>();
    }
}
