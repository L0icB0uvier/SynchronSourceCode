using System;
using Characters.CharacterAbilities.AbilitiesPower;
using Characters.Controls.Controllers.PlayerControllers.Hicks;
using Characters.Controls.Controllers.PlayerControllers.Skullface;
using GeneralScriptableObjects;
using SceneManagement.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class BoostUIManager : MonoBehaviour
	{
		private Image m_energyBar;
		
		[SerializeField] private FloatVariable current;
		[SerializeField] private FloatReference max;
		[SerializeField] private FloatReference min;
		
		public Color fullEnergyColor;
		public Color emptyEnergyColor;

		private UnityEngine.Camera m_currentCamera;

		private void Awake()
		{
			m_energyBar = GetComponent<Image>();
			HideUI();
		}

		private void HideUI()
		{
			m_energyBar.enabled = false;
		}

		private void DisplayUI()
		{
			m_energyBar.enabled = true;
		}

		private void Update()
		{
			if (Math.Abs(current.Value - max) < .05f && m_energyBar.enabled)
			{
				HideUI();
				return;
			}

			if (Math.Abs(current.Value - max) > .05f && !m_energyBar.enabled)
			{
				DisplayUI();
			}

			if (!m_energyBar.enabled) return;
			
			UpdateFill(Mathf.Clamp01(Mathf.InverseLerp(min, max, current.Value)));
		}
		
		public void UpdateFill(float fill)
		{
			m_energyBar.color = Color.Lerp(emptyEnergyColor, fullEnergyColor, fill);
			m_energyBar.fillAmount = fill;
		}

		public enum EBarOwner {Drone, Hicks}

	}
}
