using System;
using UnityEngine;

namespace RPG.Stats {
    public class BaseStats : MonoBehaviour {
        [SerializeField] private StatsContainer _stats;

        [SerializeField] private int _level;
        [SerializeField] private float _experience;

        public event Action OnLevelUp;

        public void AddExperience(float amount) {
            var exp = Stat.EXPERIENCE_TO_PROMOTE;
            _experience += amount;
            if (_experience > _stats.GetBaseStat(exp) + _stats.GetLevelStat(exp, _level)) {
                _experience = 0F;
                _level += 1;
                OnLevelUp?.Invoke();
            }
        }

        public float GetStatValue(Stat stat) {
            return _stats.GetBaseStat(stat) + _stats.GetLevelStat(stat, _level);
        }

        public float GetBaseStat(Stat stat) => _stats.GetBaseStat(stat);
    }
}
