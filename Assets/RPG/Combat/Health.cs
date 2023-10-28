using System;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using RPG.Stats;
using RPG.Utils;
using UnityEditor;
using UnityEngine;

namespace RPG.Combat {
    public class Health : MonoBehaviour, ISaveable {
        private BaseStats _stats;
        [ReadOnly] [SerializeField] private float _currentHealth = 0;
        [ReadOnly] [SerializeField] private float _maxHealth;
        public bool IsAlive => _currentHealth > 0;
        public event Action OnHitEvent;
        public event Action OnDieEvent;

        private void Awake() {
            _stats = GetComponent<BaseStats>();
        }

        private void OnEnable() {
            _stats.OnStatUpdated += OnStatUpdated;
        }
        private void OnStatUpdated() {
            _maxHealth = _stats.GetStatValue(Stat.BASE_HEALTH);
        }

        private void Start() {
            _maxHealth = _stats.GetStatValue(Stat.BASE_HEALTH);
        }

        private void Update() {
            if(_currentHealth < _maxHealth)
                _currentHealth += _stats.GetStatValue(Stat.HEALTH_REGEN) / 5 * Time.deltaTime;
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

        public JToken CaptureAsJToken() {
            var healthInfo = new JObject(new JProperty("current_health", _currentHealth));
            return healthInfo;
        }
        public void RestoreFromJToken(JToken state) {
            _currentHealth = (float)state["current_health"];
        }
    }
}
