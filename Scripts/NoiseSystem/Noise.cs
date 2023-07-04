using System;
using System.Collections;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using Lean.Pool;
using Plugins.Custom_2D_Colliders.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NoiseSystem
{
	public class Noise : MonoBehaviour
	{
		private float m_noiseDesiredRadius = 1;

		public FloatReference noiseSpeed;

		private const float StartRadius = .2f;

		public EllipseCollider2D noiseColliderShape;

		public bool StoppedByWalls { get; private set; }

		public ENoiseInstigator NoiseInstigator {private set; get;}
	
		public float EmissionTime { private set; get; }
		
		[SerializeField] private DespawnNoiseChannel despawnNoiseChannel;

		[SerializeField] private VoidEventChannelSO _onSceneTransitionStart;
		
		private void OnEnable()
		{
			_onSceneTransitionStart.onEventRaised += DespawnNoise;
		}

		private void OnDisable()
		{
			_onSceneTransitionStart.onEventRaised -= DespawnNoise;
		}

		public void StartNoiseWave(float noiseAmplitude, bool stoppedByWalls, ENoiseInstigator noiseInstigator)
		{
			EmissionTime = Time.realtimeSinceStartup;
			NoiseInstigator = noiseInstigator;
			StoppedByWalls = stoppedByWalls;
			m_noiseDesiredRadius = noiseAmplitude;
			StartCoroutine(MoveNoiseWave());
		}

		IEnumerator MoveNoiseWave()
		{
			float time = 0;
			var noiseDuration = m_noiseDesiredRadius / noiseSpeed.Value;
			
			noiseColliderShape.ChangeRadius(StartRadius);
			
			while (time <= noiseDuration)
			{
				noiseColliderShape.ChangeRadius(Mathf.Clamp(noiseColliderShape.radius + noiseSpeed.Value * Time.deltaTime, .2f,m_noiseDesiredRadius));
				time += Time.deltaTime;
				yield return null;
			}
			
			DespawnNoise();
		}
		
		private void DespawnNoise()
		{
			LeanPool.Despawn(this);
		}
	}
}
