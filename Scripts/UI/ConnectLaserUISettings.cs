using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/ConnectLaserUISettings", order = 0)]
    public class ConnectLaserUISettings : ScriptableObject
    {
        [FormerlySerializedAs("laserSocketOffset")] public float laserSocketCollisionOffset;
        public float laserSocketVisualOffset;
        public float skullfaceOffset;
    }
}