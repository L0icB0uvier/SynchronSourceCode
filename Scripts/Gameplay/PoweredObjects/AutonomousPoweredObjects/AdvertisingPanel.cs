using UnityEngine;

namespace Gameplay.PoweredObjects.AutonomousPoweredObjects
{
	public class AdvertisingPanel : PoweredSystem
	{
		[SerializeField]
		SpriteRenderer m_PanelSpriteRenderer;

		[SerializeField]
		Color m_PoweredColor;

		[SerializeField]
		Color m_UnpoweredColor;

		
		protected void Awake()
		{
			m_PanelSpriteRenderer = GetComponent<SpriteRenderer>();
		}

		public void SetPoweredColor()
		{
			m_PanelSpriteRenderer.color = m_PoweredColor;
		}

		public void SetUnpoweredColor()
		{
			m_PanelSpriteRenderer.color = m_UnpoweredColor;
		}
	}
}
