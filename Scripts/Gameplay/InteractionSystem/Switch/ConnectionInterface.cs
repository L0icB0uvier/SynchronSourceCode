using System.Collections;
using Gameplay.InteractionSystem.Interacters;
using GeneralScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Gameplay.InteractionSystem.Switch
{
    public class ConnectionInterface : MonoBehaviour
    {
        private Transform m_owner;
        
        [SerializeField] private Interacter interacter;

        [SerializeField] private FloatReference transitionTime;
        
        public ConnectionPort CurrentConnectionPort { get; private set; }

        [FoldoutGroup("Events")] public UnityEvent onInitialize;
        [FoldoutGroup("Events/MoveEvents")] public UnityEvent onInterfaceStartMoving;
        [FoldoutGroup("Events/MoveEvents")] public UnityEvent onInterfaceStopMoving;
        [FoldoutGroup("Events/ConnectionEvents")] public UnityEvent onConnectInterface;
        [FoldoutGroup("Events/ConnectionEvents")] public UnityEvent onInterfaceConnected;
        [FoldoutGroup("Events/ConnectionEvents")] public UnityEvent onDisconnectInterface;
        [FoldoutGroup("Events/InteractionEvents")] public UnityEvent onInteract;
        
        private void Awake()
        {
            m_owner = transform.root;
        }

        public void ConnectToConnectionPort(ConnectionPort connectionPort)
        {
            if (CurrentConnectionPort != null && CurrentConnectionPort != connectionPort)
            {
                CurrentConnectionPort.Disconnect(interacter);
            }
            
            transform.SetParent(connectionPort.transform, true);
            
            CurrentConnectionPort = connectionPort;

            StopAllCoroutines();
            StartCoroutine(MoveToConnectionPort());
        }
        
        private IEnumerator MoveToConnectionPort()
        {
            onInterfaceStartMoving?.Invoke();
            
            Vector2 startPosition = transform.position;
            float time = 0;
            while (time < transitionTime.Value)
            {
                time += Time.deltaTime;
                var t = Mathf.Clamp01(time / transitionTime.Value);
            
                transform.position = Vector2.Lerp(startPosition, CurrentConnectionPort.transform.position, t);
                yield return new WaitForEndOfFrame();
            }
            
            onInterfaceStopMoving?.Invoke();
            
            ConnectInterface();
        }

        private void ConnectInterface()
        {
            onConnectInterface?.Invoke();
            CurrentConnectionPort.StartConnection(interacter);
        }
    
        public void Connected()
        {
            onInterfaceConnected?.Invoke();
        }

        public bool TryInteraction()
        {
            if (CurrentConnectionPort == null || !CurrentConnectionPort.TryInteract(interacter))
            {
                return false;
            }
            
            onInteract?.Invoke();
            return true;
        }

        public void Disconnect(bool instant = false)
        {
            CurrentConnectionPort.Disconnect(interacter);
            CurrentConnectionPort = null;

            onDisconnectInterface?.Invoke();

            transform.SetParent(m_owner, true);

            if (instant)
            {
                transform.localPosition = Vector3.zero;
                return;
            }
            
            StopAllCoroutines();
            StartCoroutine(ReturnToPlayer());
        }

        private IEnumerator ReturnToPlayer()
        {
            onInterfaceStartMoving?.Invoke();
            
            Vector2 startPos = transform.localPosition;
            float time = 0;

            while (time < transitionTime)
            {
                time += Time.deltaTime;
                var t = Mathf.Clamp01(time / transitionTime);
                transform.localPosition = Vector2.Lerp(startPos, Vector2.zero, t);
                yield return new WaitForEndOfFrame();
            }
            
            onInterfaceStopMoving?.Invoke();
        }

        public void Initialize()
        {
            transform.SetParent(m_owner);
            transform.localPosition = Vector3.zero;
            CurrentConnectionPort = null;
            onInitialize?.Invoke();
        }
    }
}