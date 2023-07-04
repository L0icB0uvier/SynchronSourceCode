using System;
using System.Linq;
using Camera;
using GeneralScriptableObjects.Events;
using UnityEngine;

namespace SceneManagement
{
    public class CameraTargetTracker : MonoBehaviour
    {
        [Header("Broadcasting to")]
        [SerializeField] private TransformEventChannelSO _AddTransformToTargetGroupChannel;
        [SerializeField] private TransformEventChannelSO _RemoveTransformToTargetGroupChannel;

        [SerializeField] private string[] _acceptedTags = {"Unit"};
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_acceptedTags.Contains(other.tag))
            {
                _AddTransformToTargetGroupChannel.RaiseEvent(other.transform);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_acceptedTags.Contains(other.tag))
            {
                _RemoveTransformToTargetGroupChannel.RaiseEvent(other.transform);
            }
        }
    }
}
