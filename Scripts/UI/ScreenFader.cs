using System.Collections;
using UnityEngine;
using Utilities;

namespace UI
{
	public class ScreenFader : MonoBehaviour
	{
		[SerializeField] private FadeChannelSO m_fadeChannelSO;
		
		[SerializeField] private CanvasGroup faderCanvasGroup;
		
		private void OnEnable()
		{
			m_fadeChannelSO.OnEventRaised += InitiateFade;
		}

		private void OnDisable()
		{
			m_fadeChannelSO.OnEventRaised -= InitiateFade;
		}

		protected IEnumerator Fade(float finalAlpha, float fadeDuration)
		{
			faderCanvasGroup.blocksRaycasts = true;
			float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;
			while (!MathCalculation.ApproximatelyEqualFloat(faderCanvasGroup.alpha, finalAlpha, .05f))
			{
				faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha,
					fadeSpeed * Time.unscaledDeltaTime);
				yield return null;
			}
			faderCanvasGroup.alpha = finalAlpha;
			faderCanvasGroup.blocksRaycasts = false;

			yield return null;
		}

		public void SetAlpha(float alpha)
		{
			faderCanvasGroup.alpha = alpha;
		}
		
		private void InitiateFade(bool fadeIn, float duration)
		{
			StartCoroutine(Fade(fadeIn? 0 : 1, duration));
		}
	}
}