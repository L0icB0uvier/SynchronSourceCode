using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace UI
{
	public class ChangeTextMat : MonoBehaviour
	{
		public Material normalMat;
		public Material selectedMat;
		public Material disabledMat;

		public TMP_Text textComponent;

		[Button]
		public void ChangeToNormalMat()
		{
			textComponent.fontSharedMaterial = normalMat;
		}

		[Button]
		public void ChangeToSelectelMat()
		{
			textComponent.fontSharedMaterial = selectedMat;
		}

		[Button]
		public void ChangeToDisabledMat()
		{
			textComponent.fontSharedMaterial = disabledMat;
		}
	}
}
