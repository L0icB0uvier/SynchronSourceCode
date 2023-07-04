using System.Collections;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
	public class GameOverUIManager : MonoBehaviour
	{
		[SerializeField] private FadeChannelSO _fadeChannel;
		[SerializeField] private FloatVariable fadeDuration;
		[SerializeField] private VoidEventChannelSO gameOverScreenFinishedPlayingChannel;
		[SerializeField] private FloatEventChannelSO changeMusicVolumeEventChannel;

		[SerializeField] private Image _characterImage;
		[SerializeField] private FloatEventChannelSO changeMusicVolumeChannel;
		[SerializeField] private FloatVariable musicVolumeSetting;
		
		public void PlayGameOverScreen(Sprite characterImage, bool showImage)
		{
			changeMusicVolumeChannel.RaiseEvent(0);
			
			if (showImage)
			{
				_characterImage.sprite = characterImage;
				_characterImage.gameObject.SetActive(true);
			}

			else
			{
				_characterImage.gameObject.SetActive(false);
			}
			
			gameObject.SetActive(true);
		}
		
		//Called by animation event
		public void GameOverScreenFinishedPlaying()
		{
			StartCoroutine(FadeAndHideUI());
		}

		private IEnumerator FadeAndHideUI()
		{
			_fadeChannel.FadeOut(fadeDuration.Value);
			yield return new WaitForSecondsRealtime(fadeDuration.Value);
			gameOverScreenFinishedPlayingChannel.RaiseEvent();
			gameObject.SetActive(false);
			changeMusicVolumeEventChannel.RaiseEvent(musicVolumeSetting.Value);
		}
	}
}
