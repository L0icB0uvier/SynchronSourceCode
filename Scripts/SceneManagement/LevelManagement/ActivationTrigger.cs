using UnityEngine;

namespace SceneManagement.LevelManagement
{
	public class ActivationTrigger : MonoBehaviour
	{	
		public string[] acceptedTag = new string[1];

		public AreaManager area;

		public bool disableAfterTriggering = true;

		public void Save()
		{
			ES3.Save(GetInstanceID().ToString(), gameObject.activeInHierarchy);
		}

		public void Load()
		{
			gameObject.SetActive(ES3.Load<bool>(GetInstanceID().ToString()));
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			foreach(string tag in acceptedTag)
			{
				if (tag == collision.tag)
				{
					//UnitCentralisation.Instance.OnIntrusionDetected(area);

					if (disableAfterTriggering)
					{
						gameObject.SetActive(false);
					}
					return;
				}
			}
		
		}
	}
}
