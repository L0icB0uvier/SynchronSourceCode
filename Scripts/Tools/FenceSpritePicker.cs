using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools
{
	public class FenceSpritePicker : MonoBehaviour
	{
		public Sprite[] fancesSprite = new Sprite[0];

		public SpriteRenderer spriteRenderer;

		[Button]
		public void ChangeSprite()
		{
			if (!spriteRenderer)
			{
				spriteRenderer = GetComponent<SpriteRenderer>();
			}

			spriteRenderer.sprite = fancesSprite[Random.Range(0, fancesSprite.Length - 1)];
		}
	}
}
