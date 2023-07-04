using System;
using DG.Tweening;
using DG.Tweening.Core;
using GeneralScriptableObjects.Events;
using UnityEngine;

namespace SceneManagement.Managers
{
	public class TimeManager : MonoBehaviour
	{
		[SerializeField] private BoolEventChannelSO changeTimePausedEvent;
		[SerializeField] private ChangeTimeScaleEventChannel updateTimeScaleEventChannel;

		private bool m_gamePaused;
	
		private float m_timeScaleBeforePause;
		private float m_fixedDeltaTimeBeforePause;

		private void OnEnable()
		{
			changeTimePausedEvent.OnEventRaised += ChangeTimePause;
			updateTimeScaleEventChannel.OnEventRaised += ChangeTimeScale;
		}

		private void OnDisable()
		{
			changeTimePausedEvent.OnEventRaised -= ChangeTimePause;
			updateTimeScaleEventChannel.OnEventRaised -= ChangeTimeScale;
		}

		private void ChangeTimePause(bool timePaused)
		{
			if (timePaused)
			{
				if (m_gamePaused)
					return;

				m_gamePaused = true;
				m_timeScaleBeforePause = Time.timeScale;
				m_fixedDeltaTimeBeforePause = Time.fixedDeltaTime;
				Time.timeScale = 0;
			}

			else
			{
				if (!m_gamePaused)
					return;

				Time.timeScale = m_timeScaleBeforePause;
				Time.fixedDeltaTime = m_fixedDeltaTimeBeforePause;
				m_gamePaused = false;
			}
		}

		private void ChangeTimeScale(float newScale, bool lerp, float lerpTime)
		{
			if (Math.Abs(Time.timeScale - newScale) < .01) return;
			
			if (lerp)
			{
				DOTween.To(() => Time.timeScale, x => Time.timeScale = x, newScale, lerpTime);
				DOTween.To(() => Time.fixedDeltaTime, x => Time.fixedDeltaTime = x, Time.fixedUnscaledDeltaTime * newScale, lerpTime);
			}

			else
			{
				Time.timeScale = newScale;
				Time.fixedDeltaTime *= newScale;
			}
		}
	}
}
