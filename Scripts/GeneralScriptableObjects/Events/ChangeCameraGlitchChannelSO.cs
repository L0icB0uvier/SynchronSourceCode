using Camera;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace GeneralScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/Camera/Change Camera Glitch setting Event Channel")]
    public class ChangeCameraGlitchChannelSO : ScriptableObject
    {
        public event UnityAction<EGlitchSettingType, float> OnEventRaised;

        public void RaiseEvent(EGlitchSettingType settingTypeType, float value)
        {
            OnEventRaised?.Invoke(settingTypeType, value);
        }
    }
}