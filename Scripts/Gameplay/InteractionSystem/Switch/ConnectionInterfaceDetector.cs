using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.InteractionSystem.Switch
{
   public abstract class ConnectionInterfaceDetector : MonoBehaviour
   {
      public string[] _tags;

      private readonly List<ConnectionInterfaceManager> m_overlappingInterfaceManagers = new List<ConnectionInterfaceManager>();
      private readonly List<ConnectionInterfaceManager> m_registeredConnectionInterfaces = new List<ConnectionInterfaceManager>();

      [SerializeField] private ConnectionPort _connectionPort;

      private bool m_connectionAuthorized = true;

      public UnityEvent onConnectionPossible;
      public UnityEvent onConnectionImpossible;

      public void Initialize()
      {
         m_registeredConnectionInterfaces.Clear();
         _connectionPort.Initialize();
      }

      public void AuthorizeConnection()
      {
         m_connectionAuthorized = true;
         onConnectionPossible?.Invoke();
         
         RegisterPendingInterfaceManager();
      }
      

      public void ForbidConnection()
      {
         m_connectionAuthorized = false;
         onConnectionImpossible?.Invoke();
         
         SetInterfaceManagersToPending();
      }
      
      private void RegisterPendingInterfaceManager()
      {
         if (m_overlappingInterfaceManagers.Count <= 0) return;
         foreach (var interfaceManager in m_overlappingInterfaceManagers)
         {
            RegisterInterfaceManager(interfaceManager);
         }
         
         m_overlappingInterfaceManagers.Clear();
      }

      private void SetInterfaceManagersToPending()
      {
         if (m_registeredConnectionInterfaces.Count <= 0) return;

         for (int i = m_registeredConnectionInterfaces.Count - 1; i >= 0; i--)
         {
            RegisterOverlappingInterfaceManager(m_registeredConnectionInterfaces[i]);
            UnregisterInterfaceManager(m_registeredConnectionInterfaces[i]);
         }
      }

      private void RegisterInterfaceManager(ConnectionInterfaceManager connectionInterfaceManager)
      {
         if (m_registeredConnectionInterfaces.Contains(connectionInterfaceManager)) return;
         m_registeredConnectionInterfaces.Add(connectionInterfaceManager);
         connectionInterfaceManager.EnterInteractionPort(_connectionPort);
      }

      private void UnregisterInterfaceManager(ConnectionInterfaceManager connectionInterfaceManager)
      {
         if (m_registeredConnectionInterfaces.Count <= 0 || !m_registeredConnectionInterfaces.Contains(connectionInterfaceManager)) return;
         connectionInterfaceManager.ExitInteractionPort(_connectionPort);
         m_registeredConnectionInterfaces.Remove(connectionInterfaceManager);
      }
   
      private void OnTriggerEnter2D(Collider2D other)
      {
         if (!IsTagAccepted(other.transform.root.tag)) return;
         
         var connectionInterfaceManager = FindInterfaceManager(other.transform.root);
         if (connectionInterfaceManager == null) return;

         if (!m_connectionAuthorized)
         {
            RegisterOverlappingInterfaceManager(connectionInterfaceManager);
         }

         else
         {
            RegisterInterfaceManager(FindInterfaceManager(other.transform.root));
         }
      }
      
      private void OnTriggerExit2D(Collider2D other)
      {
         if (!IsTagAccepted(other.transform.root.tag)) return;
         
         var connectionInterfaceManager = FindInterfaceManager(other.transform.root);
         if (connectionInterfaceManager == null) return;
         
         if (!m_connectionAuthorized)
         {
            UnregisterOverlappingInterfaceManager(connectionInterfaceManager);
         }

         else
         {
            UnregisterInterfaceManager(connectionInterfaceManager);
         }
      }
      
      private void RegisterOverlappingInterfaceManager(ConnectionInterfaceManager connectionInterfaceManager)
      {
         if (m_overlappingInterfaceManagers.Contains(connectionInterfaceManager)) return;
         m_overlappingInterfaceManagers.Add(connectionInterfaceManager);
      }

      private void UnregisterOverlappingInterfaceManager(ConnectionInterfaceManager connectionInterfaceManager)
      {
         if (!m_overlappingInterfaceManagers.Contains(connectionInterfaceManager)) return;
         m_overlappingInterfaceManagers.Remove(connectionInterfaceManager);
      }

      private ConnectionInterfaceManager FindInterfaceManager(Transform objectTransformRoot)
      {
         var socleManager = objectTransformRoot.GetComponentInChildren<ConnectionInterfaceManager>();
         
         if (socleManager == null)
         {
            Debug.LogError(objectTransformRoot.gameObject.name + " doesn't have a socle component.");
         }

         return socleManager;
      }
      
      private bool IsTagAccepted(string colliderRootTag)
      {
         return _tags.Contains(colliderRootTag);
      }
   }
}
