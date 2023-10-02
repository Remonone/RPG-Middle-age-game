using System;
using RPG.Stats;
using RPG.Utils;
using UnityEditor;
using UnityEngine;

namespace RPG.Combat {
    public class Health : MonoBehaviour {
        private BaseStats _stats;

        [ReadOnly] [SerializeField] private float _currentHealth = 0;

        public bool IsAlive => _currentHealth > 0;

        public event Action OnHitEvent;
        public event Action OnDieEvent;

        private void Awake() {
            _stats = GetComponent<BaseStats>();
        }

        private void Start() {
            _currentHealth = _stats.GetStatValue(Stat.BASE_HEALTH);
        }

        public void HitEntity(DamageReport report) {
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
