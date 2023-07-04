using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Gameplay.Triggers
{
	public class ElectrifiedGround : MonoBehaviour
	{
		[SerializeField]
		private EElectricEffectType electricEffectType;
	
		[ShowIf("IsFixeIntermittence")]
		[SerializeField] private float fixedDelay;
	
		[MinMaxSlider(0,5)][ShowIf("IsRandomIntermittence")]
		[SerializeField] private Vector2 randomDelay;

		[SerializeField][ShowIf("IsNotConstant")]
		private float cycleOffset = 0;

		public UnityEvent onElectricArcStart;
		public UnityEvent onElectricArcStop;

		private Animator m_animator;
		private static readonly int intermittent = Animator.StringToHash("Intermittent");
		private static readonly int playElectricArc = Animator.StringToHash("PlayElectricArc");

		private bool IsNotConstant()
		{
			return electricEffectType != EElectricEffectType.Constant;
		}
	
		private bool IsFixeIntermittence()
		{
			return electricEffectType == EElectricEffectType.FixeIntermittence;
		}
	
		private bool IsRandomIntermittence()
		{
			return electricEffectType == EElectricEffectType.RandomIntermittence;
		}
	
		private void Awake()
		{
			m_animator = GetComponent<Animator>();
		}

		private void Start()
		{
			if (electricEffectType == EElectricEffectType.Constant)
			{
				m_animator.SetBool(intermittent, false);
				StartElecticArc();
				return;
			}
		
			m_animator.SetBool(intermittent, true);
			if (cycleOffset > 0) StartCoroutine(Delay(cycleOffset));
			else m_animator.SetTrigger(playElectricArc);
		}

		public void StartElecticArc()
		{
			onElectricArcStart?.Invoke();
		}

		public void ArcAnimationCompleted()
		{
			onElectricArcStop?.Invoke();
			switch (electricEffectType)
			{
				case EElectricEffectType.Constant:
					break;
				case EElectricEffectType.FixeIntermittence:
					StartCoroutine(Delay(fixedDelay));
					break;
			
				case EElectricEffectType.RandomIntermittence:
					StartCoroutine(Delay(Random.Range(randomDelay.x, randomDelay.y)));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		IEnumerator Delay(float t)
		{
			yield return new WaitForSeconds(t);
			OnDelayOver();
		}

		private void OnDelayOver()
		{
			m_animator.SetTrigger(playElectricArc);
		}
	}

	public enum EElectricEffectType {Constant, FixeIntermittence, RandomIntermittence}
}