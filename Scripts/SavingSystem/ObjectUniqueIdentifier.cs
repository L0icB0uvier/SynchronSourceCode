using System;
using SceneManagement.Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SavingSystem
{
    public class ObjectUniqueIdentifier : MonoBehaviour
    {
        [ReadOnly]
        public string id = Guid.NewGuid().ToString();

        [Button]
        public void GenerateID()
        {
            id = Guid.NewGuid().ToString();
        }
    }
}