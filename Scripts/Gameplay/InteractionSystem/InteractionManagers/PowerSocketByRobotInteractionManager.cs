using System;
using Gameplay.EnergySystem.EnergyProduction;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.InteractionSystem.InteractionManagers
{
    public class PowerSocketByRobotInteractionManager : MonoBehaviour
    {
        private EnergySocket m_socket;

        public UnityEvent onInteractionPossible;
        public UnityEvent onInteractionImpossible;

        private void Awake()
        {
            m_socket = GetComponent<EnergySocket>();
        }

        public void ResolveState()
        {
            switch (m_socket.socketState)
            {
                case ESocketState.Unpowered:
                    if (m_socket.RobotMovingToSocket)
                    {
                        onInteractionImpossible?.Invoke();
                    }

                    else
                    {
                        onInteractionPossible?.Invoke();
                    }
                    break;
                case ESocketState.PoweredByEnergyCell:
                    onInteractionImpossible?.Invoke();
                    break;
                case ESocketState.PoweredByRobot:
                    if (m_socket.PoweringRobot != null)
                    {
                        onInteractionPossible?.Invoke();
                    }

                    else
                    {
                        onInteractionImpossible?.Invoke();
                    }
                    break;
            }
        }

        public void EnableInteraction()
        {
            enabled = true;
            ResolveState();
        }

        public void DisableInteraction()
        {
            enabled = false;
            onInteractionImpossible?.Invoke();
        }
    }
}