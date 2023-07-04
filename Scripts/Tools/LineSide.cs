using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
	public class LineSide : MonoBehaviour
	{
		public Transform lineStart;

		public Transform lineEnd;

		Collider2D m_Trigger;

		public List<SpriteRenderer> affectedRenderers = new List<SpriteRenderer>();

		public string[] affectedTag = new string[0];

		private void Awake()
		{
			m_Trigger = GetComponent<Collider2D>();
		}

		void FixedUpdate()
		{
			if (affectedRenderers.Count == 0)
				return;

			foreach(SpriteRenderer affectedRenderer in affectedRenderers)
			{
				if(affectedRenderer == null)
				{
					continue;
				}

				float factor = Mathf.Sign((lineStart.position.x - affectedRenderer.transform.root.position.x) * (lineEnd.position.y - affectedRenderer.transform.root.position.y) - (lineStart.position.y - affectedRenderer.transform.root.position.y) * (lineEnd.position.x - affectedRenderer.transform.root.position.x));
				if (factor == -1)
					affectedRenderer.sortingLayerName = "AboveMain";	
				else
					affectedRenderer.sortingLayerName = "UnderMain";
			}	
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			foreach(string tag in affectedTag)
			{
				if(tag == collision.tag)
				{
					switch (collision.tag)
					{
						case "Hicks":
							affectedRenderers.Add(collision.transform.Find("Sprites/HicksSprite").GetComponent<SpriteRenderer>());
							break;
						case "PlayerDrone":
							affectedRenderers.Add(collision.transform.Find("Sprites/Drone").GetComponent<SpriteRenderer>());
							break;
						case "Enemy":
							affectedRenderers.Add(collision.transform.Find("Sprites/Enemy").GetComponent<SpriteRenderer>());
							break;
						default:
							break;
					}
				}
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			foreach (string tag in affectedTag)
			{
				if (tag == collision.tag)
				{
					switch (collision.tag)
					{
						case "Hicks":
							SpriteRenderer hicks = collision.transform.Find("Sprites/HicksSprite").GetComponent<SpriteRenderer>();
							hicks.sortingLayerName = "Main";
							affectedRenderers.Remove(hicks);
							break;
						case "PlayerDrone":
							SpriteRenderer drone = collision.transform.Find("Sprites/Drone").GetComponent<SpriteRenderer>();
							drone.sortingLayerName = "Main";
							affectedRenderers.Remove(drone);
							break;
						case "Enemy":
							SpriteRenderer enemy = collision.transform.Find("Sprites/Enemy").GetComponent<SpriteRenderer>();
							enemy.sortingLayerName = "Main";
							affectedRenderers.Remove(enemy);
							break;
						default:
							break;
					}
				}
			}
		}
	}
}
