using UnityEditor;
using UnityEngine;

namespace SceneManagement.NavigationPoints
{
	public class GuardPoint : MonoBehaviour
	{
		public BehaviorAtLocation behaviorAtLocation;

#if UNITY_EDITOR

		private void OnDrawGizmos()
		{
			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.red;
			Handles.Label(transform.position + new Vector3(5, 5, 0), gameObject.name, style);
		}

		private void OnDrawGizmosSelected()
		{
			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.blue;
			Handles.Label(transform.position + new Vector3(5, 5, 0), gameObject.name, style);
		}

#endif
	}
}