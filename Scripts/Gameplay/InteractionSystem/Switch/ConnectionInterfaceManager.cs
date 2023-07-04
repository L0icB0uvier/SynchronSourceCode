using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.InteractionSystem.Switch
{
    public class ConnectionInterfaceManager : MonoBehaviour
    {
        [SerializeField] private ConnectionInterface _connectionInterface;
        public ConnectionInterface ConnectionInterface => _connectionInterface;

        [SerializeField] private List<ConnectionPort> m_connectionPortsAvailable = new List<ConnectionPort>();

        public ConnectionPort CurrentConnectionPort { get; private set; }

        private void Update()
        {
            if (m_connectionPortsAvailable.Count == 0) return;
            
            var nearestInteractionPort = FindNearestInteractionPort();

            if (nearestInteractionPort == null) return;
            
            if (nearestInteractionPort != CurrentConnectionPort)
            {
                ConnectSocle(nearestInteractionPort);
            }
        }

        public void EnterInteractionPort(ConnectionPort connectionPort)
        {
            if (m_connectionPortsAvailable.Contains(connectionPort)) return;
            m_connectionPortsAvailable.Add(connectionPort);

            if (m_connectionPortsAvailable.Count == 1 && !connectionPort.InteractionInterfaceConnected)
            {
                ConnectSocle(connectionPort);
            }
        }

        public void ExitInteractionPort(ConnectionPort connectionPort)
        {
            if (!m_connectionPortsAvailable.Contains(connectionPort)) return;
            if(CurrentConnectionPort == connectionPort) DisconnectSocle();
            m_connectionPortsAvailable.Remove(connectionPort);
        }

        private void ConnectSocle(ConnectionPort connectionPort)
        {
            CurrentConnectionPort = connectionPort;
            _connectionInterface.ConnectToConnectionPort(connectionPort);
        }

        public void DisconnectSocle()
        {
            if (CurrentConnectionPort == null) return;
            CurrentConnectionPort = null;
            _connectionInterface.Disconnect();
        }

        private ConnectionPort FindNearestInteractionPort()
        {
            var availablePorts = m_connectionPortsAvailable.FindAll(x => (!x.InteractionInterfaceConnected || x.ConnectedInterfaceManager == this));

            if (availablePorts.Count == 0) return null;
            if (availablePorts.Count == 1) return availablePorts[0];

            availablePorts = availablePorts.OrderBy(x => (x.transform.position - transform
            .position).sqrMagnitude).ToList();
            return availablePorts[0];
        }

        public void Initialize()
        {
            m_connectionPortsAvailable.Clear();
            if (CurrentConnectionPort != null)
            {
                _connectionInterface.Disconnect(true);
            }
            
            CurrentConnectionPort = null;
        }

        public void DisableConnectionInterface()
        {
            Initialize();
            _connectionInterface.Initialize();
            _connectionInterface.gameObject.SetActive(false);
            enabled = false;
        }

        public void EnableConnectionInterface()
        {
            _connectionInterface.gameObject.SetActive(true);
            enabled = true;
        }
    }
}