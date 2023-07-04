using System.Linq;
using UnityEngine;

namespace Characters.DamageSystem
{
    public class ElectricalDamage : Damager
    {
        private Animator m_animator;

        protected override void Awake()
        {
            base.Awake();
            m_animator = GetComponent<Animator>();
        }

        protected override void NonDamageableHit()
        {
            throw new System.NotImplementedException();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (damagedTags.Contains(other.tag))
            {
                Damageable damageable = other.GetComponent<Damageable>();

                damageable.TakeDamage(this, Vector2.zero);
            
            }
        }
    }
}
