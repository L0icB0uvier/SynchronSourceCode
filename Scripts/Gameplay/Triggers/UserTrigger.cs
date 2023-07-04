using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Triggers
{
	public class UserTrigger : MonoBehaviour
	{
		public string[] triggeringTags = new string[1];

		public UnityEvent onTriggerEnter;

		public UnityEvent onTriggerExit;

		[SerializeField]
		private int m_userCount;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (!triggeringTags.Contains(collision.tag)) return;
			if(m_userCount == 0) onTriggerEnter?.Invoke();
		
			m_userCount++;
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (!triggeringTags.Contains(collision.tag)) return;
			m_userCount--;
		
			if (m_userCount == 0) onTriggerExit?.Invoke();
		}
	}
}
