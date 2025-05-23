﻿using System;
using Newtonsoft.Json.Linq;
using RPG.Combat.DamageDefinition;
using RPG.Core.Predicate.Interfaces;
using RPG.Saving;
using RPG.Stats;
using RPG.Utils;
using Unity.Netcode;
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
    public class Health : NetworkBehaviour, ISaveable, IPredicateHandler {

        private LazyValue<float> _maxHealth;
        private LazyValue<float> _currentHealth;
        
        public NetworkVariable<float> _targetNetHealth = new();
        public NetworkVariable<float> _targetNetMaxHealth = new();
        [SerializeField] private Animator _animator;

        private BaseStats _stats;
        public Action<DamageReport> OnHit;
        public Action<float> OnHealthChanged;
        public Action OnDie;
        private static readonly int Hit = Animator.StringToHash("OnHit");
        private static readonly int Dead = Animator.StringToHash("IsDead");
        public bool IsAlive => _currentHealth.Value > 0;
        public float CurrentHealth => _currentHealth.Value;
        public float MaxHealth => _maxHealth.Value;

        private void OnCurrentHealthChange(float oldValue, float newValue) {
            _targetNetHealth.Value = newValue;
        }
        private void OnMaxHealthChange(float oldValue, float newValue) {
            _targetNetMaxHealth.Value = newValue;
        }
        private float OnHealthInit() {
            return _stats.GetStatValue(Stat.BASE_HEALTH);
        }

        private void Awake() {
            _stats = GetComponent<BaseStats>();
            _maxHealth = new LazyValue<float>(OnHealthInit);
            _currentHealth = new LazyValue<float>(OnHealthInit);
        }

        public override void OnNetworkSpawn() {
            if (IsServer) {
                _maxHealth.OnValueChanged += OnMaxHealthChange;
                _currentHealth.OnValueChanged += OnCurrentHealthChange;
            }
            if (!IsOwner) return;
            TrackHealthChangeServerRpc();
        }
        [ServerRpc]
        private void TrackHealthChangeServerRpc() {
            _stats.OnStatUpdated += OnStatUpdated;
            OnStatUpdated();
        }

        public object Predicate(string command, object[] arguments) {
            return command switch {
                "GetCurrentHealth" => _currentHealth.Value,
                "GetTotalHealth" => _maxHealth.Value,
                _ => "" 
            };
        }
        private void OnStatUpdated() {
            _maxHealth.Value = _stats.GetStatValue(Stat.BASE_HEALTH);
            if (_currentHealth.Value == 0) _currentHealth.Value = _maxHealth.Value;
        }
        
        private void Update() {
            if (!IsAlive) return;
            if(_currentHealth.Value < _maxHealth.Value)
                _currentHealth.Value += _stats.GetStatValue(Stat.HEALTH_REGEN) / 5 * Time.deltaTime;
            OnHealthChanged?.Invoke(_currentHealth.Value);
        }


        public void HitEntity(DamageReport report) {
            if (!IsServer) return;
            // Scaling resistance percent from actual resist(can explain later)
            var resistanceScale = 1 / (1 + Math.Pow(2, -_stats.GetStatValue((Stat)(int)report.Type))); 
            _currentHealth.Value = (float)Math.Max(_currentHealth.Value - report.Damage * resistanceScale, 0);
            OnHit?.Invoke(report);
            OnHealthChanged?.Invoke(_currentHealth.Value);
            _animator.SetTrigger(Hit);
            if (_currentHealth.Value <= 0) Die();
            GetHitClientRpc();
        }

        [ClientRpc]
        private void GetHitClientRpc(ClientRpcParams clientRpcParams = default) {
            _animator.SetTrigger(Hit);
            if (_currentHealth.Value <= 0) Die();
        }
        
        private void Die() {
            _animator.SetBool(Dead, true);
            OnDie?.Invoke();
        }

        public JToken CaptureAsJToken() {
            var healthInfo = new JObject(new JProperty("current_health", _currentHealth.Value));
            return healthInfo;
        }
        public void RestoreFromJToken(JToken state) {
            _currentHealth.Value = (float)state["current_health"];
        }
    }
}
