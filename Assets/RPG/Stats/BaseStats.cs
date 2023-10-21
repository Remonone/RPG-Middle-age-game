using System;
using Newtonsoft.Json.Linq;
using RPG.Combat.Buffs;
using RPG.Combat.Modifiers;
using RPG.Saving;
using RPG.Utils;
using UnityEngine;

namespace RPG.Stats {
    [RequireComponent(typeof(BuffContainer))]
    public class BaseStats : MonoBehaviour, ISaveable {
        [SerializeField] private StatsContainer _stats;

        [SerializeField] private int _level;
        [SerializeField] private float _experience;

        public BuffContainer Buffs;

        public event Action OnLevelUp;

        private void Awake() {
            Buffs = GetComponent<BuffContainer>();
        }
        
        public void AddExperience(float amount) {
            var exp = Stat.EXPERIENCE_TO_PROMOTE;
            _experience += amount;
            if (_experience > _stats.GetBaseStat(exp) + _stats.GetLevelStat(exp, _level)) {
                _experience = 0F;
                _level += 1;
                OnLevelUp?.Invoke();
            }
        }
        
        public float GetBaseStat(Stat stat) => _stats.GetBaseStat(stat);

        public float GetStatValue(Stat stat) {
            return (_stats.GetBaseStat(stat) + _stats.GetLevelStat(stat, _level) + CalculateFlatStatChangers(stat)) * CalculatePercentStatChangers(stat);
        }
        private float CalculatePercentStatChangers(Stat stat) {
            IStatModifier[] statChangers = GetComponents<IStatModifier>();
            float totalValue = 1f;
            
            foreach (var part in statChangers) {
                totalValue += part.ReflectFlatStat(stat);
            }

            return totalValue;
        }

        private float CalculateFlatStatChangers(Stat stat) {
            IStatModifier[] statChangers = GetComponents<IStatModifier>();
            float totalValue = 0f;
            
            foreach (var part in statChangers) {
                totalValue += part.ReflectPercentStat(stat);
            }

            return totalValue;
        }
        #if UNITY_EDITOR
        
        private void Start() {
            UpdateInfo();
        }
        void UpdateInfo() {
            _info.UpdateInfo(GetStatValue(Stat.BASE_HEALTH), GetStatValue(Stat.PHYSICAL_RESISTANCE), GetStatValue(Stat.HEALTH_REGEN));
        }
        [SerializeField] private ReadOnlyStats _info;

        
        [Serializable]
        internal sealed class ReadOnlyStats {
            [ReadOnly] public float _baseHealth;
            [ReadOnly] public float _basePhysicalResist;
            [ReadOnly] public float _baseRegen;
            public void UpdateInfo(float baseHealth, float baseResist, float baseRegen) {
                _baseHealth = baseHealth;
                _basePhysicalResist = baseResist;
                _baseRegen = baseRegen;
            }
        }
        #endif
        public JToken CaptureAsJToken() {
            return new JObject(
                new JProperty("level", _level),
                new JProperty("experience", _experience)
            );
        }
        public void RestoreFromJToken(JToken state) {
            _level = (int)state["level"];
            _experience = (int)state["experience"];
        }
    }
    
}
