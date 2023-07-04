using UI;
using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/LaserEvents/Connect Laser Event Channel")]
    public class ConnectLaserEventChannelSO : ScriptableObject
    {
        public event UnityAction<ConnectLaserUI> OnEventRaised;

        public void RaiseEvent(ConnectLaserUI value)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(value);
        }
    }
}