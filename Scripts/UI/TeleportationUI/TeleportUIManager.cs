using System.Collections;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace UI.TeleportationUI
{
	public class TeleportUIManager : MonoBehaviour
	{
		private Transform m_hicksTransform;
		private Transform m_skullfaceTransform;
		
		[SerializeField] private LineRenderer lineRenderer;

		[SerializeField] private ParticleSystem sightBlockedParticleSystem;

		[SerializeField] private TeleportationUISettings settings;
		
		private static readonly int dispGlitchOn = Shader.PropertyToID("_DispGlitchOn");
		private static readonly int colorGlitchOn = Shader.PropertyToID("_ColorGlitchOn");

		[SerializeField] private VoidEventChannelSO[] disableTeleportUIChannels;
		[SerializeField] private VoidEventChannelSO[] enableTeleportUIChannels;

		[SerializeField] private VoidEventChannelSO teleportFailedChannel;

		[SerializeField] private UnityEvent onDisplayUI;
		[SerializeField] private UnityEvent onHideUI;
		
		private Vector2 m_lineDirection;

		private bool m_uiVisible;

		private Coroutine m_teleportationFailedFeedbackEffect;
		private Coroutine m_fadeLineCoroutine;

		[SerializeField] private Material tpVisualMaterial;
		[SerializeField] private Material followVisualMaterial;

		[SerializeField] private BoolVariable followPossible;

		private void Awake()
		{
			lineRenderer = GetComponent<LineRenderer>();
			
			m_hicksTransform = GameObject.FindGameObjectWithTag("Hicks").transform;
			m_skullfaceTransform = GameObject.FindGameObjectWithTag("Skullface").transform;
			
			foreach (var channel in disableTeleportUIChannels)
			{
				channel.onEventRaised += DisableTeleportationUI;
			}

			foreach (var channel in enableTeleportUIChannels)
			{
				channel.onEventRaised += EnableTeleportationUI;
			}
		}

		private void OnEnable()
		{
			teleportFailedChannel.onEventRaised += OnTeleportFailed;
		}

		private void OnDisable()
		{
			teleportFailedChannel.onEventRaised -= OnTeleportFailed;
		}
		
		private void OnDestroy()
		{
			foreach (var channel in disableTeleportUIChannels)
			{
				channel.onEventRaised -= DisableTeleportationUI;
			}

			foreach (var channel in enableTeleportUIChannels)
			{
				channel.onEventRaised -= EnableTeleportationUI;
			}
		}

		private void Start()
		{
			HideUIElements();
		}

		private void Update()
		{
			if (ShouldDisplayLine(m_hicksTransform.transform.position, m_skullfaceTransform.transform.position))
			{
				UpdateLineMaterial();
				
				if (!m_uiVisible)
				{
					ShowUIElements();
				}
				
				ManageTeleportUI();
			}

			else
			{
				if(m_uiVisible)
				{
					HideUIElements();
				}
			}
		}

		private void EnableTeleportationUI()
		{
			SetLineStartEndPos();
			gameObject.SetActive(true);
		}

		private void DisableTeleportationUI()
		{
			StopAllCoroutines();
			m_fadeLineCoroutine = null;
			gameObject.SetActive(false);
		}

		private void HideUIElements()
		{
			onHideUI?.Invoke();
			m_uiVisible = false;
			StopAllCoroutines();
			m_fadeLineCoroutine = null;
		}

		private void ShowUIElements()
		{
			onDisplayUI?.Invoke();
			m_uiVisible = true;
			m_fadeLineCoroutine = StartCoroutine(FadeTpLine());
		}

		private void ManageTeleportUI()
		{
			SetLineStartEndPos();

			if (IsLineOfSightBlocked() || IsHicksAboveJammer() || IsSkullfaceAboveJammer())
			{
				ChangeLineEffect(false);
			}

			else
			{
				if(!EnvironmentalQueryUtilities.IsOnGround(m_skullfaceTransform.transform.position))
				{
					//lineRenderer.material.SetFloat(dispGlitchOn, 0);
					//lineRenderer.material.SetFloat(colorGlitchOn, 0);
					lineRenderer.startColor = settings.tpImpossibleColor;
					lineRenderer.endColor = settings.tpPossibleColor;
					return;
				}
				
				ChangeLineEffect(true);
			}
		}

		private void SetLineStartEndPos()
		{
			Vector2 hicksPos = m_hicksTransform.transform.position;
			Vector2 skullfacePos = m_skullfaceTransform.transform.position;

			m_lineDirection = (skullfacePos - hicksPos).normalized;
			Vector2 startPos = hicksPos + m_lineDirection * settings.lineStartVisualOffset;
			Vector2 endPos = skullfacePos + -m_lineDirection * settings.lineStartVisualOffset;

			lineRenderer.SetPosition(0, startPos + new Vector2(0, settings.tpVisualYOffset));
			lineRenderer.SetPosition(1, endPos + new Vector2(0, settings.tpVisualYOffset));
		}

		private bool ShouldDisplayLine(Vector2 startPos, Vector2 endPos)
		{
			return ((startPos - endPos).sqrMagnitude > settings.showUIMinDistance * settings.showUIMinDistance);
		}

		private bool IsLineOfSightBlocked()
		{
			var hit = Physics2D.Linecast(m_hicksTransform.transform.position, m_skullfaceTransform.transform.position, 
			settings.teleportObstacleLayerMask.value);

			if (hit)
			{
				sightBlockedParticleSystem.gameObject.transform.position = hit.point;
				if(!sightBlockedParticleSystem.isPlaying) sightBlockedParticleSystem.Play();
				return true;
			}
			
			if(sightBlockedParticleSystem.isPlaying) sightBlockedParticleSystem.Stop();
			return false;
		}

		private bool IsHicksAboveJammer()
		{
			if(EnvironmentalQueryUtilities.IsInsideJammer(m_hicksTransform.transform.position))
			{
				return true;
			}
			
			return false;
		}
		
		private bool IsSkullfaceAboveJammer()
		{
			if (EnvironmentalQueryUtilities.IsInsideJammer(m_skullfaceTransform.transform.position))
			{
				return true;
			}
			
			return false;
		}

		private void ChangeLineEffect(bool tpPossible)
		{
			if (tpPossible)
			{
				lineRenderer.startColor = settings.tpPossibleColor;
				lineRenderer.endColor = settings.tpPossibleColor;
				//lineRenderer.material.SetFloat(dispGlitchOn, 0);
				//lineRenderer.material.SetFloat(colorGlitchOn, 0);
			}

			else
			{
				lineRenderer.startColor = settings.tpImpossibleColor;
				lineRenderer.endColor = settings.tpImpossibleColor;
				//lineRenderer.material.SetFloat(dispGlitchOn, 1);
				//lineRenderer.material.SetFloat(colorGlitchOn, 1);
			}
		}

		private void UpdateLineMaterial()
		{
			lineRenderer.material = followPossible.Value ? followVisualMaterial : tpVisualMaterial;
		}

		private IEnumerator FadeTpLine()
		{
			float t = 0;
			while (true)
			{
				var a = Mathf.Lerp(settings.fadeMinAlpha, settings.fadeMaxAlpha, Mathf.Abs(Mathf.Sin(t * settings.fadeSpeed)));
				settings.tpImpossibleColor.a = a;
				settings.tpPossibleColor.a = a;
				t += Time.deltaTime;
				yield return null;
			}
		}
		
		private void OnTeleportFailed()
		{
			if (m_teleportationFailedFeedbackEffect != null) return;
			StopAllCoroutines();
			m_teleportationFailedFeedbackEffect = StartCoroutine(TeleportationFailed());
		}

		private IEnumerator TeleportationFailed()
		{
			float t = 0;
			while (t < settings.teleportationFailedFeedbackDuration)
			{
				t += Time.deltaTime;
				var f = Mathf.Clamp01(t / settings.teleportationFailedFeedbackDuration);

				var currentAlpha = settings.tpImpossibleColor.a;
				
				var a = Mathf.Lerp(currentAlpha, settings.teleportFailedAlpha, f);
				settings.tpImpossibleColor.a = a;

				lineRenderer.widthMultiplier = Mathf.Lerp(settings.defaultWidthMultiplier, settings
				.teleportFailedWidthMultiplier, f);
				yield return null;
			}

			t = 0;
			
			while (t < settings.teleportationFailedFeedbackDuration)
			{
				t += Time.deltaTime;
				var f = Mathf.Clamp01(t / settings.teleportationFailedFeedbackDuration);
				
				var a = Mathf.Lerp(settings.teleportFailedAlpha, settings.fadeMinAlpha, f);
				settings.tpImpossibleColor.a = a;
				
				lineRenderer.widthMultiplier = Mathf.Lerp(settings
					.teleportFailedWidthMultiplier, settings.defaultWidthMultiplier, f);
				yield return null;
			}

			m_teleportationFailedFeedbackEffect = null;
			m_fadeLineCoroutine = StartCoroutine(FadeTpLine());
		}
	}
}
