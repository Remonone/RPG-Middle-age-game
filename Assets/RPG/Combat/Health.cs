using System;
using Newtonsoft.Json.Linq;
using RPG.Core.Predicate;
using RPG.Saving;
using RPG.Stats;
using RPG.Utils;
using UnityEngine;

namespace RPG.Combat {
    /// <summary>
    /// <list type="bullet">
    ///     <listheader>EventList:</listheader>
    ///     <item>
    ///         <term>OnHitEvent: </term>
    ///         <description>event which invokes on hit. Args: DamageReport</description>
    ///     </item>
    ///     <item>
    ///         <term>OnDieEvent: </term>
    ///         <description>event which invokes whenever target is dead.</description>
    ///     </item>
    /// </list> 
    /// </summary>
    public class Health : PredicateMonoBehaviour, ISaveable {
        private BaseStats _stats;
        [ReadOnly] [SerializeField] private float _currentHealth;
        [ReadOnly] [SerializeField] private float _maxHealth;
        public bool IsAlive => _currentHealth > 0;
        
        protected override void OnAwake() {
            _stats = GetComponent<BaseStats>();
        }

        protected override void OnEnableEvent() {
            _stats.OnStatUpdated += OnStatUpdated;
        }
        public override void Predicate(string command, object[] arguments, out object result) {
            result = command switch {
                "GetCurrentHealth" => _currentHealth,
                "GetTotalHealth" => _maxHealth,
                _ => "" 
            };
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
            Storage.InvokeEvent("OnHitEvent", report);
            if (_currentHealth <= 0) Die();
        }
        private void Die() {
            Storage.InvokeEvent("OnDieEvent");
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
