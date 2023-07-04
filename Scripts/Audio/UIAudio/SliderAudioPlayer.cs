using Audio.SFXPlayers;
using UnityEngine;

namespace Audio.UIAudio
{
    public class SliderAudioPlayer : SFXAudioPlayer
    {
        [SerializeField] private AudioCueSO sliderAudioCue;
        [SerializeField] private AudioCueSO sliderSelectedAudioCue;

        public void PlaySliderAudio() => PlayAudio(sliderAudioCue);
        public void PlaySliderSelectedAudio() => PlayAudio(sliderSelectedAudioCue);
    }
}