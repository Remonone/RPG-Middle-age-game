using System;
using Newtonsoft.Json.Linq;
using RPG.Combat.DamageDefinition;
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
        [SerializeField] private Animator _animator;

        public Action<DamageReport> OnHit;
        public Action OnDie;
        private static readonly int Hit = Animator.StringToHash("OnHit");
        private static readonly int Dead = Animator.StringToHash("IsDead");
        public bool IsAlive => _currentHealth > 0;
        
        protected override void OnAwake() {
            _stats = GetComponent<BaseStats>();
        }

        protected override void OnEnableEvent() {
            _stats.OnStatUpdated += OnStatUpdated;
        }
        public override object Predicate(string command, object[] arguments) {
            return command switch {
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
            _currentHealth = _maxHealth;
        }

        private void Update() {
            if (!IsAlive) return;
            if(_currentHealth < _maxHealth)
                _currentHealth += _stats.GetStatValue(Stat.HEALTH_REGEN) / 5 * Time.deltaTime;
        }

        public void HitEntity(DamageReport report) {
            // Scaling resistance percent from actual resist(can explain later)
            var resistanceScale = 1 / (1 + Math.Pow(2, -_stats.GetStatValue((Stat)(int)report.Type))); 
            _currentHealth = (float)Math.Max(_currentHealth - report.Damage * resistanceScale, 0);
            OnHit?.Invoke(report);
            _animator.SetTrigger(Hit);
            if (_currentHealth <= 0) Die();
        }
        private void Die() {
            _animator.SetBool(Dead, true);
            OnDie?.Invoke();
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
