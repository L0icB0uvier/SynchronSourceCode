using Gameplay.InteractionSystem.Interactables;
using Gameplay.InteractionSystem.Interacters;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.InteractionSystem.Switch
{
    public class ConnectionPort : MonoBehaviour
    {
        public UnityEvent<ConnectionPort> onConnectConnectionInterface;
        public UnityEvent onConnectionInterfaceConnected;
        public UnityEvent<ConnectionPort> onDisconnectConnectionInterface;
        
        public bool InteractionInterfaceConnected { get; private set; }
        public ConnectionInterfaceManager ConnectedInterfaceManager { get; private set; }
        
        public UnityEvent onConnectionPortEnabled;
        public UnityEvent onConnectionPortDisabled;

        [SerializeField] private Interactable interactable;

        public bool TryInteract(Interacter interacter)
        {
            return interactable.TryInteraction(interacter);
        }

        public void StartConnection(Interacter interacter)
        {
            interactable.InteracterEntered(interacter);
            InteractionInterfaceConnected = true;
            ConnectedInterfaceManager = interacter.ConnectionInterfaceManager;
            onConnectConnectionInterface?.Invoke(this);
        }

        public void Disconnect(Interacter interacter)
        {
            interactable.InteracterExited(interacter);
            InteractionInterfaceConnected = false;
            onDisconnectConnectionInterface?.Invoke(this);
        }

        public void PortConnected()
        {
            onConnectionInterfaceConnected?.Invoke();
        }

        public void EnableConnectionPort()
        {
            onConnectionPortEnabled?.Invoke();
        }

        public void DisableConnectionPort()
        {
            onConnectionPortDisabled?.Invoke();
        }

        public void Initialize()
        {
            
        }
    }
}