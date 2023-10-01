using System;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat {
    public class Health : MonoBehaviour {
        private BaseStats _stats;

        private float _currentHealth;

        public bool IsAlive => _currentHealth > 0;

        public event Action OnHitEvent;
        public event Action OnDieEvent;

        private void Awake() {
            _stats = GetComponent<BaseStats>();
        }

        public void Hit(DamageReport report) {
            // Scaling resistance percent from actual resist(can explain later)
            var resistanceScale = 1 / (1 + Math.Pow(2, -_stats.GetStatValue((Stat)(int)report.Type))); 
            _currentHealth = (float)Math.Max(_currentHealth - report.Damage * resistanceScale, 0);
            OnHitEvent?.Invoke();
            if (_currentHealth <= 0) Die();
        }
        private void Die() {
            OnDieEvent?.Invoke();
        }

    }
}
