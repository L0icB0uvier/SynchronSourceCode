using System.Collections;
using GeneralScriptableObjects;
using UnityEngine;

namespace Characters.Enemies.Perception
{
    public class InvestigateSign : MonoBehaviour
    {
        [SerializeField] private FloatVariable investigateSignDuration;
    
        private void OnEnable()
        {
            StartCoroutine(DisableAfterDelay());
        }

        private IEnumerator DisableAfterDelay()
        {
            yield return new WaitForSeconds(investigateSignDuration.Value);
            gameObject.SetActive(false);
        }
    }
}
