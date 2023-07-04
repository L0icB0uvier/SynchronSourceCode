using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.EnergySystem.EnergyProduction
{
   public class RobotPowerDetector : MonoBehaviour
   {
      [SerializeField] private string[] poweringTags;

      private GameObject m_poweringUnit;

      public UnityEvent<GameObject> onPoweringRobotEntered;
      public UnityEvent onPoweringRobotExited;

      private void OnTriggerEnter2D(Collider2D other)
      {
         if(m_poweringUnit != null || !poweringTags.Contains(other.tag)) return;

         m_poweringUnit = other.gameObject;
         onPoweringRobotEntered?.Invoke(m_poweringUnit);
      }

      private void OnTriggerExit2D(Collider2D other)
      {
         if((m_poweringUnit != null && other.gameObject != m_poweringUnit) || !poweringTags.Contains(other.tag)) return;
         m_poweringUnit = null;
      
         onPoweringRobotExited?.Invoke();
      }
   }
}
