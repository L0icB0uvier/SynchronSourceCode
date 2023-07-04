using UnityEditor;
using UnityEngine;

namespace SceneManagement.NavigationPoints
{
	public class RestPoint : MonoBehaviour
	{
#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (Selection.activeGameObject == gameObject)
				return;

			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.red;
			Handles.Label(transform.position + new Vector3(2, 2, 0), gameObject.name, style);
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
