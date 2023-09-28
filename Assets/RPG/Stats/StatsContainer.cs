using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Stats {
    [CreateAssetMenu(fileName = "New Stats", menuName = "GumuPeachu/Create New Stats", order = 0)]
    public class StatsContainer : ScriptableObject {
        
        [SerializeField] private string _raceName;
        // Uuuhhh...

        private readonly int _statCount = Enum.GetNames(typeof(Stat)).Length;
        
        [SerializeField] private StatValue[] _baseStats = {
            new() {stat = Stat.BASE_HEALTH, value = 1},
            new() {stat = Stat.PHYSICAL_RESISTANCE, value = 1},
            new() {stat = Stat.MAGICAL_RESISTANCE, value = 1},
            new() {stat = Stat.PALE_RESISTANCE, value = 1},
            new() {stat = Stat.CHAOTIC_RESISTANCE, value = 1},
            new() {stat = Stat.MANA, value = 1},
            new() {stat = Stat.MOVEMENT_SPEED, value = 1},
            new() {stat = Stat.BASE_ATTACK, value = 1},
            new() {stat = Stat.CRITICAL_CHANCE, value = 1},
            new() {stat = Stat.PP_ATTACK_MODIFIER, value = 1},
            new() {stat = Stat.MC_ATTACK_MODIFIER, value = 1},
            new() {stat = Stat.ATTACK_RANGE, value = 1},
            new() {stat = Stat.EXPERIENCE_TO_PROMOTE, value = 1},
        };
        [SerializeField] private StatValue[] _scaleStats = {
            new() {stat = Stat.BASE_HEALTH, value = 1},
            new() {stat = Stat.PHYSICAL_RESISTANCE, value = 1},
            new() {stat = Stat.MAGICAL_RESISTANCE, value = 1},
            new() {stat = Stat.PALE_RESISTANCE, value = 1},
            new() {stat = Stat.CHAOTIC_RESISTANCE, value = 1},
            new() {stat = Stat.MANA, value = 1},
            new() {stat = Stat.MOVEMENT_SPEED, value = 1},
            new() {stat = Stat.BASE_ATTACK, value = 1},
            new() {stat = Stat.CRITICAL_CHANCE, value = 1},
            new() {stat = Stat.PP_ATTACK_MODIFIER, value = 1},
            new() {stat = Stat.MC_ATTACK_MODIFIER, value = 1},
            new() {stat = Stat.ATTACK_RANGE, value = 1},
            new() {stat = Stat.EXPERIENCE_TO_PROMOTE, value = 1},
        };

        private Dictionary<Stat, float> _stats = new Dictionary<Stat, float>();
        private Dictionary<Stat, float> _scales = new Dictionary<Stat, float>();
    
        public void InitStats() {
            foreach (string str in Enum.GetNames(typeof(Stat))) {
                var characteristic = _baseStats.Single(value => value.stat.ToString() == str);
                var scaleValue = _baseStats.Single(value => value.stat.ToString() == str);
                _stats[characteristic.stat] = characteristic.value;
                _scales[scaleValue.stat] = scaleValue.value;
            }
        }
        
        public void OnValidate() {
            if (_baseStats.Length != _statCount || _scaleStats.Length != _statCount) 
                throw new NullReferenceException("Initialization isn't fulfilled");
        }

        public float GetBaseStat(Stat stat) {
            return _stats[stat];
        }

        public float GetLevelStat(Stat stat, int level) {
            return _scales[stat] * level;
        }
    }

    [Serializable]
    internal class StatValue {
        public Stat stat;
        public float value;
    }
}
