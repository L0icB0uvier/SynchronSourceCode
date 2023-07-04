using Audio;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    [RequireComponent(typeof(Slider))]
    public class VolumeSliderManager : MonoBehaviour
    {
        private AudioManager m_audioManager;
        private Slider m_slider;
        [SerializeField] private string volumeParamName;
    
        private void Awake()
        {
            m_audioManager = FindObjectOfType<AudioManager>();
            m_slider = GetComponent<Slider>();
        }

        private void OnEnable()
        {
            m_slider.value =  m_audioManager.GetGroupVolume(volumeParamName);
        }
    }
}
