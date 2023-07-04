using System.Collections.Generic;
using Characters.Controls.Controllers.PlayerControllers;
using UnityEngine;

namespace SceneManagement.LevelManagement
{
	[System.Serializable]
	public class TargetInfo
	{
		public PlayerController targetController;

		public bool currentlyVisible;

		public Vector2 lastKnownLocation;

		public bool lastLocationChecked;

		public Vector2 targetMovingDirection;

		List<GameObject> m_UnitsWithVisual = new List<GameObject>();
		public List<GameObject> UnitsWithVistual => m_UnitsWithVisual;

		public TargetInfo(PlayerController target)
		{
			targetController = target;
		}

		public void UpdateLastKnownLocation()
		{
			if (currentlyVisible)
			{
				lastLocationChecked = false;
				lastKnownLocation = targetController.transform.position;
				targetMovingDirection = targetController.Rb2d.velocity.normalized;
			}
		}

		public void UnitGainVisual(GameObject unit)
		{
			m_UnitsWithVisual.Add(unit);
			currentlyVisible = true;
		}

		public void UnitLostVisual(GameObject unit)
		{
			m_UnitsWithVisual.Remove(unit);

			if(m_UnitsWithVisual.Count == 0)
			{
				currentlyVisible = false;
			}
		}
	}
}