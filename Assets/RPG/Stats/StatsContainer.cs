using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Combat;
using UnityEngine;

namespace RPG.Stats {
    [CreateAssetMenu(fileName = "New Stats", menuName = "GumuPeachu/Create New Stats", order = 0)]
    public class StatsContainer : ScriptableObject, ISerializationCallbackReceiver {
        
        [SerializeField] private CreatureClass _class;
        
        [SerializeField] private List<StatValue> _baseStats = new();
        [SerializeField] private List<StatValue> _scaleStats = new();


        public CreatureClass CreatureClass => _class; 
        
        public float GetBaseStat(Stat stat) {
            return _baseStats.Single(basic => basic.stat == stat).value;
        }

        public float GetLevelStat(Stat stat, int level) {
            return _scaleStats.Single(basic => basic.stat == stat).value * level;
        }

        public void OnBeforeSerialize() {
            if (_baseStats.Count == 0 && _scaleStats.Count == 0) {
                foreach (var str in Enum.GetNames(typeof(Stat))) {
                    _baseStats.Add(new StatValue{stat = Enum.Parse<Stat>(str), value = 1});
                    _scaleStats.Add(new StatValue{stat = Enum.Parse<Stat>(str), value = 1});
                }
            }
        }
        public void OnAfterDeserialize() { }
    }

    [Serializable]
    internal class StatValue {
        public Stat stat;
        public float value;
    }
}
