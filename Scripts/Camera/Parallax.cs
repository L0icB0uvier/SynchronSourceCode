using SceneManagement;
using SceneManagement.Camera;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Camera
{
	public class Parallax : MonoBehaviour
	{
		[Range(0,99)]
		public float movingSpeed = .1f;

		[SerializeField]
		private Vector2 m_cameraOffset;

		private FollowCamera m_followCamera;

		private UnityEngine.Camera m_camera;

		private void Awake()
		{
			m_followCamera = GetComponentInParent<FollowCamera>();
			m_followCamera.onCameraMoved += UpdateParallax;
			m_camera = UnityEngine.Camera.main;
		}

		private void UpdateParallax()
		{
			var newLocalPos = (m_cameraOffset + (-(Vector2) m_followCamera.transform.position)) * movingSpeed;
			transform.localPosition = new Vector3(newLocalPos.x,newLocalPos.y, transform.localPosition.z);
		}

		[Button]
		public void SaveOffset()
		{
			m_cameraOffset = transform.parent.position;
		}
	}
}
