using GeneralScriptableObjects.Events;
using UnityEngine;

namespace Camera
{
    public class GlitchManager : MonoBehaviour
    {
        private GlitchCameraShader m_glitchShader;

        [SerializeField] private VoidEventChannelSO enableGlitchChannels;
        [SerializeField] private VoidEventChannelSO disableGlitchChannels;

        [SerializeField] private ChangeCameraGlitchChannelSO changeCameraGlitchSettingChannel;
        
        private void Awake()
        {
            m_glitchShader = GetComponent<GlitchCameraShader>();
        }

        private void OnEnable()
        {
            enableGlitchChannels.onEventRaised += EnableGlitchShader;
            disableGlitchChannels.onEventRaised += DisableGlitchShader;
            
            changeCameraGlitchSettingChannel.OnEventRaised += ChangeGlitchSetting;
        }

        private void OnDisable()
        {
           
            enableGlitchChannels.onEventRaised -= EnableGlitchShader;
            disableGlitchChannels.onEventRaised -= DisableGlitchShader;
            
            changeCameraGlitchSettingChannel.OnEventRaised -= ChangeGlitchSetting;
            
            m_glitchShader.enabled = false;
        }

        private void Start()
        {
            m_glitchShader.enabled = false;
        }

        private void EnableGlitchShader()
        {
            if (m_glitchShader.enabled) return;
            
            m_glitchShader.enabled = true;
        }

        private void DisableGlitchShader()
        {
            if (!m_glitchShader.enabled) return;
            
            m_glitchShader.enabled = false;
        }

        private void ChangeGlitchSetting(EGlitchSettingType settingType, float value)
        {
            switch (settingType)
            {
                case EGlitchSettingType.Rate:
                    ChangeGlitchRate(value);
                    break;
                case EGlitchSettingType.IntensityShift:
                    ChangeGlitchRGBShiftIntensity(value);
                    break;
                case EGlitchSettingType.Interval:
                    ChangeGlitchInterval(value);
                    break;
                case EGlitchSettingType.NoiseIntensity:
                    ChangeGlitchNoiseIntensity(value);
                    break;
            }
        }

        private void ChangeGlitchNoiseIntensity(float newIntensity)
        {
            m_glitchShader.WhiteNoiseIntensity = newIntensity;
        }

        private void ChangeGlitchInterval(float newInterval)
        {
            m_glitchShader.GlitchInterval = newInterval;
        }

        private void ChangeGlitchRate(float newRate)
        {
            m_glitchShader.GlitchRate = newRate;
        }

        private void ChangeGlitchRGBShiftIntensity(float newRate)
        {
            m_glitchShader.RGBShiftIntensity = newRate;
        }
    }
}