using UnityEngine;
using UnityEngine.Events;

namespace SceneManagement.LevelManagement
{
	public class EventTrigger : MonoBehaviour
	{
		public string[] acceptedTag = new string[1];

		public bool disableAfterTriggering = true;

		public UnityEvent OnTriggerEnter;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			foreach (string tag in acceptedTag)
			{
				if (!collision.CompareTag(tag)) continue;
				
				OnTriggerEnter.Invoke();

				if (disableAfterTriggering)
				{
					gameObject.SetActive(false);
				}

				return;
			}
		}
	}
}
