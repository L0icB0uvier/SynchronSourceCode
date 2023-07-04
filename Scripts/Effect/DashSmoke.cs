using UnityEngine;

namespace Effect
{
	public class DashSmoke : MonoBehaviour
	{
		public void AnimationComplete()
		{
			gameObject.SetActive(false);
		}
   
	}
}
