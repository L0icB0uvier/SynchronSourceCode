using System;
using GeneralScriptableObjects.Events;
using UnityEngine;

namespace SceneManagement
{
    public class AudioListenerManager : MonoBehaviour
    {
        [SerializeField] private Vector2EventChannelSO _onCameraPositionChanged;

        private void OnEnable()
        {
            throw new NotImplementedException();
        }

        private void OnDisable()
        {
            throw new NotImplementedException();
        }
    }
}