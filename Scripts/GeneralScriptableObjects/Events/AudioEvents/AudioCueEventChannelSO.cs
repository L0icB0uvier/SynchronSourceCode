﻿using GeneralScriptableObjects;
using UnityEngine;

namespace GeneralScriptableObjects.Events
{
	/// <summary>
	/// Event on which <c>AudioCue</c> components send a message to play SFX and music. <c>AudioManager</c> listens on these events, and actually plays the sound.
	/// </summary>
	[CreateAssetMenu(menuName = "Events/AudioCue Event Channel")]
	public class AudioCueEventChannelSO : ScriptableObject
	{
		public AudioCuePlayAction OnAudioCuePlayRequested;
		public MovingAudioCuePlayAction OnMovingAudioCuePlayRequested;
		public AudioCueStopAction OnAudioCueStopRequested;
		public AudioCueFadeOutAction OnAudioCueFadeOutRequested;
		public AudioCueFinishAction OnAudioCueFinishRequested;
		public AudioCueChangeVolumeAction OnChangeVolumeRequested;

		public AudioCueKey RaisePlayEvent(AudioCueSO audioCue, AudioConfigurationSO audioConfiguration, Vector3 positionInSpace = default)
		{
			AudioCueKey audioCueKey = AudioCueKey.Invalid;

			if (OnAudioCuePlayRequested != null)
			{
				audioCueKey = OnAudioCuePlayRequested.Invoke(audioCue, audioConfiguration, positionInSpace);
			}
			else
			{
				Debug.LogWarning("An AudioCue play event was requested  for " + audioCue.name +", but nobody picked it up. " +
				                 "Check why there is no AudioManager already loaded, " +
				                 "and make sure it's listening on this AudioCue Event channel.");
			}

			return audioCueKey;
		}

		public bool RaiseStopEvent(AudioCueKey audioCueKey)
		{
			bool requestSucceed = false;

			if (OnAudioCueStopRequested != null)
			{
				requestSucceed = OnAudioCueStopRequested.Invoke(audioCueKey);
			}
			else
			{
				Debug.LogWarning("An AudioCue stop event was requested, but nobody picked it up. " +
				                 "Check why there is no AudioManager already loaded, " +
				                 "and make sure it's listening on this AudioCue Event channel.");
			}

			return requestSucceed;
		}

		public bool RaiseFadeOutEvent(AudioCueKey audioCueKey, float fadeDuration)
		{
			bool requestSucceed = false;

			if (OnAudioCueFadeOutRequested != null)
			{
				requestSucceed = OnAudioCueFadeOutRequested.Invoke(audioCueKey, fadeDuration);
			}
			else
			{
				Debug.LogWarning("An AudioCue stop event was requested, but nobody picked it up. " +
				                 "Check why there is no AudioManager already loaded, " +
				                 "and make sure it's listening on this AudioCue Event channel.");
			}

			return requestSucceed;
		}

		public void RaiseChangeVolumeEvent(AudioCueKey audioCueKey, float newVolume)
		{
			OnChangeVolumeRequested?.Invoke(audioCueKey, newVolume);
		}

		public bool RaiseFinishEvent(AudioCueKey audioCueKey)
		{
			bool requestSucceed = false;

			if (OnAudioCueStopRequested != null)
			{
				requestSucceed = OnAudioCueFinishRequested.Invoke(audioCueKey);
			}
			else
			{
				Debug.LogWarning("An AudioCue finish event was requested, but nobody picked it up. " +
				                 "Check why there is no AudioManager already loaded, " +
				                 "and make sure it's listening on this AudioCue Event channel.");
			}

			return requestSucceed;
		}
	}

	public delegate AudioCueKey AudioCuePlayAction(AudioCueSO audioCue, AudioConfigurationSO audioConfiguration, Vector3 positionInSpace);
	public delegate AudioCueKey MovingAudioCuePlayAction(AudioCueSO audioCue, AudioConfigurationSO audioConfiguration, Transform followedTransform);
	public delegate bool AudioCueStopAction(AudioCueKey emitterKey);
	public delegate bool AudioCueFadeOutAction(AudioCueKey emitterKey, float fadeDuration);
	public delegate void AudioCueChangeVolumeAction(AudioCueKey emitterKey, float newVolume);
	public delegate bool AudioCueFinishAction(AudioCueKey emitterKey);
}