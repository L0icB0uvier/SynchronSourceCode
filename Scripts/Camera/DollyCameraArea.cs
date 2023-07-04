using Cinemachine;
using UnityEngine;

namespace Camera
{
	public class DollyCameraArea : MonoBehaviour
	{
		CinemachineVirtualCamera m_VirtualCam;

		private void Awake()
		{
			m_VirtualCam = GetComponentInChildren<CinemachineVirtualCamera>();
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			//if(collision.tag == "Hicks")
			//{
			//	CameraManager.Instance.EnterDollyCamArea(m_VirtualCam);
			//}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			//if (collision.tag == "Hicks")
			//{
			//	CameraManager.Instance.ExitDollyCamArea(m_VirtualCam);
			//}
		}
	}
}
