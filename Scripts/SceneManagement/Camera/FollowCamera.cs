using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace SceneManagement.Camera
{
    [ExecuteAlways]
    public class FollowCamera : MonoBehaviour
    {
        private UnityEngine.Camera m_camera;

        public UnityAction onCameraMoved;
    
        private void Awake()
        {
            m_camera = FindObjectOfType<UnityEngine.Camera>();
        }

        private void Update()
        {
#if UNITY_EDITOR
            MatchCameraPosition();
#endif
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            MatchCameraPosition();
        }

        [Button]
        public void MatchCameraPosition()
        {
            if (!m_camera)
            {
                m_camera = FindObjectOfType<UnityEngine.Camera>();
            }

            var camPosition = m_camera.transform.position;
       
            transform.position = new Vector3(camPosition.x, camPosition.y, 0);
            onCameraMoved?.Invoke();
        }
    }
}
