using System.Collections;
using System.Text;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Tools
{
	public class NumberGenerator : MonoBehaviour
	{
		public bool isDecimal;

		public TextMeshProUGUI m_Text;

		[Range(1, 20)]
		public int numberSize = 4;
		StringBuilder generatedString;

		public float generationRate = 1;

		private void OnEnable()
		{
			StartCoroutine(GenerateNumber());
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}

		IEnumerator GenerateNumber()
		{
			while (true)
			{
				Generate();

				yield return new WaitForSecondsRealtime(generationRate);
			}
		}

		[Button]
		public void Generate()
		{
			string generatedNumber = string.Empty;

			if (isDecimal)
			{
				generatedNumber += "0.";
			}

			for (int i = 0; i < numberSize; i++)
			{
				generatedNumber += Random.Range(0, 9);
			}

			m_Text.SetText(generatedNumber);
		}
	}
}
