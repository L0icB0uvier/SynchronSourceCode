using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class EnergyUIManager : MonoBehaviour
	{
		public Image energyBar;

		public void UpdateBarFill(float newFill)
		{
			energyBar.fillAmount = newFill;
		}
	}
}
