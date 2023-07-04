using System.Collections;
using Cinemachine;
using GeneralScriptableObjects.Events;
using UnityEngine;

public class VirtualCameraManager : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO[] resetCameraListenedChannel;

    [SerializeField] private CinemachineVirtualCamera defaultCamera;
    private CinemachineVirtualCamera[] m_virtualCameras;

    private GameObject[] m_fixeCameraTriggers;

    private void Awake()
    {
        m_virtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();

        foreach (var channel in resetCameraListenedChannel)
        {
            channel.onEventRaised += ResetVirtualCameras;
        }

        m_fixeCameraTriggers = GameObject.FindGameObjectsWithTag("FixeCameraTrigger");

    }

    private void OnDestroy()
    {
        foreach (var channel in resetCameraListenedChannel)
        {
            channel.onEventRaised -= ResetVirtualCameras;
        }
    }

    private void ResetVirtualCameras()
    {
        StartCoroutine(ResetCameras());
    }

    private IEnumerator ResetCameras()
    {
        foreach (var cameraTrigger in m_fixeCameraTriggers)
        {
            cameraTrigger.SetActive(false);
        }
        
        foreach (var vc in m_virtualCameras)
        {
            vc.Priority = 0;
        }
        
        yield return null;
        
        defaultCamera.Priority = 1;

        yield return null;

        foreach (var cameraTrigger in m_fixeCameraTriggers)
        {
            cameraTrigger.SetActive(true);
        }
    }
}
