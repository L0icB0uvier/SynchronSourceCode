using System.Collections;
using GeneralScriptableObjects;
using UnityEngine;

namespace Characters.CharacterAbilities.AbilitiesPower
{
    public class AbilitiesPower : MonoBehaviour, IAbilitiesPower
    {
        [SerializeField] private FloatVariable currentPower;

        [SerializeField] private FloatReference maxPower;
        public float MaxPower => maxPower;
        public bool IsRecharging { get; set; }
        public bool IsEmpty => currentPower.Value == 0;

        [SerializeField] private FloatReference powerRechargeRate;
        [SerializeField] private FloatReference timeBeforeRecharge;
        [SerializeField] private BoolVariable aboveVoid;
        
        private Coroutine m_rechargeTimer;
        private Coroutine m_rechargeEnergy;

        private void Start()
        {
            currentPower.SetValue(maxPower.Value);
        }

        public void ConsumePower(float powerConsumed)
        {
            currentPower.SetValue(Mathf.Clamp(currentPower.Value - powerConsumed, 0, maxPower.Value));
            if (m_rechargeEnergy != null)
            {
                StopCoroutine(m_rechargeEnergy);
                IsRecharging = false;
            }
            if(m_rechargeTimer != null) StopCoroutine(m_rechargeTimer);
            m_rechargeTimer = StartCoroutine(RechargeTimer());
        }

        private IEnumerator RechargeTimer()
        {
            yield return new WaitForSeconds(timeBeforeRecharge);

            while (aboveVoid.Value)
            {
                yield return null;
            }
            
            m_rechargeEnergy = StartCoroutine(RechargeEnergy());
        }

        private IEnumerator RechargeEnergy()
        {
            IsRecharging = true;
            
            while (currentPower.Value < maxPower)
            {
                currentPower.SetValue(Mathf.Clamp(currentPower.Value + powerRechargeRate * Time.deltaTime, 0, maxPower));
                yield return null;
            }

            IsRecharging = false;
        }
    }
}