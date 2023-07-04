using System.Linq;
using UnityEngine;

namespace Utilities
{
   public static class PrefabInstantiationUtility
   {
      public static PrefabInstantiationData m_Data;

      public static GameObject GetGameObjectRefByName(string prefabName)
      {
         if(m_Data == null)
         {
            SetData();
         
            if (m_Data == null)
            {
               Debug.LogError("PrefabInstantiationData could not be found at provided path.");
               return null;
            }
         }
         
         if (!IsPrefabNameInDatabase(prefabName)) return null;
         
         return m_Data.prefabs.First(x => x.name == prefabName);
      }

      private static void SetData()
      {
         m_Data = Resources.Load<PrefabInstantiationData>("Utilities/PrefabInstantiationData");
      }
      
      private static bool IsPrefabNameInDatabase(string prefabName)
      {
         if (m_Data.prefabs.All(x => x.name != prefabName))
         {
            Debug.LogError("Cannot instantiate prefab called " + prefabName +
                           " because there is no prefab with that name in scriptable object PrefabInstantiationData.");
            return false;
         }

         return true;
      }
   }
}
