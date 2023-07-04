using Cinemachine;
using GeneralScriptableObjects.Events;
using SceneManagement;
using SceneManagement.Camera;
using UnityEngine;
using UnityEngine.Serialization;

namespace Camera
{
	public class CameraManager : MonoBehaviour
	{
		private CinemachineBrain m_brain;
		
		[SerializeField] private GameObject m_virtualCameras;
	
		public CinemachineVirtualCamera currentCam;
	
		public CinemachineVirtualCamera m_followCamera;

		private CinemachineTargetGroup m_mainTargetGroup;
	
		private FollowCamera m_paralax;
		
		[Header("Listening to")]
		[SerializeField] private TransformEventChannelSO _AddTransformToTargetGroupChannel;
		[SerializeField] private TransformEventChannelSO _RemoveTransformToTargetGroupChannel;

		[SerializeField] private VoidEventChannelSO[] enableCameraBrainChannel;
		[SerializeField] private VoidEventChannelSO[] disableCameraBrainChannel;
		
		[SerializeField] private ChangeCameraBlendUpdateMethodEventChannel changeCameraBlendUpdateMethod;
		[SerializeField] private Vector3EventChannelSO warpCameraEventChannel;
		
		
		private void Awake()
		{
			m_brain = GetComponent<CinemachineBrain>();
			m_virtualCameras = GameObject.FindWithTag("VirtualCameras");

			Initialise();
			
			foreach (var channel in enableCameraBrainChannel)
			{
				channel.onEventRaised += EnableCameraBrain;
			}
	        
			foreach (var channel in disableCameraBrainChannel)
			{
				channel.onEventRaised += DisableCameraBrain;
			}
		}

		private void OnEnable()
		{
			_AddTransformToTargetGroupChannel.OnEventRaised += AddObjectToFollow;
			_RemoveTransformToTargetGroupChannel.OnEventRaised += RemoveObjectToFollow;
			warpCameraEventChannel.onEventRaised += WarpCamera;
			changeCameraBlendUpdateMethod.OnEventRaised += ChangeCameraBlendUpdateMethod;
		}

		
		private void OnDisable()
		{
			_AddTransformToTargetGroupChannel.OnEventRaised -= AddObjectToFollow;
			_RemoveTransformToTargetGroupChannel.OnEventRaised -= RemoveObjectToFollow;
			warpCameraEventChannel.onEventRaised -= WarpCamera;
			changeCameraBlendUpdateMethod.OnEventRaised -= ChangeCameraBlendUpdateMethod;
		}

		private void OnDestroy()
		{
			foreach (var channel in enableCameraBrainChannel)
			{
				channel.onEventRaised -= EnableCameraBrain;
			}
	        
			foreach (var channel in disableCameraBrainChannel)
			{
				channel.onEventRaised -= DisableCameraBrain;
			}
		}

		private void OnDrawGizmos()
		{
			if (Application.isPlaying) return;
		
			if (transform.hasChanged)
			{
				if (!m_paralax)
				{
					m_paralax = FindObjectOfType<FollowCamera>();
					if (!m_paralax) return;
				}
			
				m_paralax.MatchCameraPosition();
			}
		}

		public void Initialise()
		{
			m_virtualCameras = GameObject.Find("VirtualCameras");
			ChangeCameraBlendUpdateMethod(CinemachineBrain.BrainUpdateMethod.FixedUpdate);

			m_followCamera = m_virtualCameras.transform.Find("FollowCamera").GetComponent<CinemachineVirtualCamera>();

			currentCam = m_followCamera;

			Transform targetGroup = m_virtualCameras.transform.Find("TargetGroup");

			if (targetGroup)
			{
				m_mainTargetGroup = targetGroup.GetComponent<CinemachineTargetGroup>();
				AddObjectToFollow(GameObject.FindGameObjectWithTag("Hicks").transform);
				AddObjectToFollow(GameObject.FindGameObjectWithTag("Skullface").transform);
			}
		}

		private void Start()
		{
			currentCam = m_followCamera;
			m_followCamera.Priority = 2;
		}
		
		private void ChangeCameraBlendUpdateMethod(CinemachineBrain.BrainUpdateMethod newBlendMethod)
		{
			m_brain.m_BlendUpdateMethod = newBlendMethod;
		}

		public void WarpCamera(Vector3 deltaMovement)
		{
			currentCam.OnTargetObjectWarped(m_mainTargetGroup.transform, deltaMovement);
		}

		public void AddObjectToFollow(Transform objectToFollow)
		{
			if (m_mainTargetGroup.FindMember(objectToFollow) == -1)
				m_mainTargetGroup.AddMember(objectToFollow, 1, 0);
		}
		
		public void RemoveObjectToFollow(Transform objectToUnfollow)
		{
			if (m_mainTargetGroup.FindMember(objectToUnfollow) == -1)
				return;

			m_mainTargetGroup.RemoveMember(objectToUnfollow);
		}

		private void EnableCameraBrain()
		{
			m_brain.enabled = true;
			m_brain.ManualUpdate();
		}

		private void DisableCameraBrain()
		{
			m_brain.enabled = false;
		}
	}
}