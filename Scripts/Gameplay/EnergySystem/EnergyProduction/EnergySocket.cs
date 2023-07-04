using System.Collections.Generic;
using Gameplay.InteractionSystem.Switch;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.EnergySystem.EnergyProduction
{
	public class EnergySocket : EnergySource
	{
		[FoldoutGroup("Settings")]
		public ESocketState socketState = ESocketState.Unpowered;

		private bool m_socketOpened;

		[FoldoutGroup("Settings")]
		public bool isMonitored;
	
		[FoldoutGroup("Settings")][ShowIf("isMonitored")][Indent()]
		public ESocketState monitoredState;

		[FoldoutGroup("Events")] public UnityEvent onOpenSocket;

		[FoldoutGroup("Events")]
		public UnityEvent<bool> onSocketOpened;

		[FoldoutGroup("Events")]
		public UnityEvent onCloseSocket;
		
		[FoldoutGroup("Events")]
		public UnityEvent onEnergyCellPlugged;

		[FoldoutGroup("Events")]
		public UnityEvent onEnergyCellUnplugged;
		
		[FoldoutGroup("Events")]
		public UnityEvent onRobotStartPowering; 
		
		[FoldoutGroup("Events")]
		public UnityEvent onRobotStopPowering;
		
		[FoldoutGroup("Events")]
		public UnityEvent onSocketStateChanged;

		private List<ConnectionPort> m_portConnectedCount = new List<ConnectionPort>();

		public bool RobotMovingToSocket { get; private set; }
		public GameObject PoweringRobot { get; private set; }

		//Called by animation event
		public void SocketOpened()
		{
			m_socketOpened = true;
			onSocketOpened?.Invoke(socketState == ESocketState.Unpowered);
		}

		//Called by animation event
		public void SocketClosed()
		{
			m_socketOpened = false;
			onCloseSocket?.Invoke();
		}
		
		public void EnergyCellPlugged()
		{
			socketState = ESocketState.PoweredByEnergyCell;
			if(m_socketOpened) onEnergyCellPlugged?.Invoke();
			onSocketStateChanged?.Invoke();
			StartProducingPower();
		}

		public void EnergyCellUnplugged()
		{
			socketState = ESocketState.Unpowered;
			if(m_socketOpened) onEnergyCellUnplugged?.Invoke();
			onSocketStateChanged?.Invoke();
			StopProducingPower();
		}

		public void PowerByRobotStarted()
		{
			RobotMovingToSocket = true;
			onRobotStartPowering?.Invoke();
		}

		public void PoweredByRobot(GameObject poweringRobot)
		{
			socketState = ESocketState.PoweredByRobot;
			RobotMovingToSocket = false;
			PoweringRobot = poweringRobot;
			onSocketStateChanged?.Invoke();
			StartProducingPower();
		}

		public void UnpoweredByRobot()
		{
			socketState = ESocketState.Unpowered;
			PoweringRobot = null;
			if(m_socketOpened) onRobotStopPowering?.Invoke();
			onSocketStateChanged?.Invoke();
			StopProducingPower();
		}

		public void PortConnected(ConnectionPort port)
		{
			if (m_portConnectedCount.Contains(port)) return;
			if (m_portConnectedCount.Count == 0)
			{
				onOpenSocket?.Invoke();
			}
			
			m_portConnectedCount.Add(port);

		}

		public void PortDisconnected(ConnectionPort port)
		{
			if (!m_portConnectedCount.Contains(port)) return;
			m_portConnectedCount.Remove(port);
			if (m_portConnectedCount.Count == 0)
			{
				onCloseSocket?.Invoke();
			}
		}

		public void Initialize()
		{
			m_socketOpened = false;
			monitoredState = ESocketState.Unpowered;
			onSocketStateChanged?.Invoke();
		}
	}
}
