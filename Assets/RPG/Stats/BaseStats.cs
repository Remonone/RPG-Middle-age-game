using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using RPG.Combat;
using RPG.Combat.Buffs;
using RPG.Core.Predicate;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats {
    [RequireComponent(typeof(BuffContainer))]
    public class BaseStats : PredicateMonoBehaviour, ISaveable {
        [SerializeField] private StatsContainer _stats;

        [SerializeField] private int _level;
        [SerializeField] private float _experience;

        public BuffContainer Buffs;
        

        public event Action OnLevelUp;
        public event Action OnStatUpdated;
        
        //TODO: CHANGE IT TO MODIFICATIONS
        private List<PredicatedStats> _predicatedStats = new List<PredicatedStats>();
        private List<PredicatedStats> _temporaryStats = new List<PredicatedStats>();
        

        protected override void OnAwake() {
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
        
        public override void Predicate(string command, object[] arguments, out object result) {
            result = command switch {
                "AmplifyStat" => AmplifyStat(arguments),
                "CancelStat" => CancelStat(arguments),
                _ => null
            };
            if (result != null) OnStatUpdated?.Invoke();
        }
        
        private bool AmplifyStat(object[] args) {
            ValidateArgs(args, typeof(string), typeof(float), typeof(float), typeof(bool));
            for (int i = 0; i < args.Length; i++) args[i] = Convert.ToString(args[i]);
            var stats = new PredicatedStats {
                Stat = (Stat)Enum.Parse(typeof(Stat), Convert.ToString(args[0])),
                FlatValue = (float)Convert.ToDouble(args[1]),
                PercentValue = (float)Convert.ToDouble(args[2])
            };
            if(Convert.ToInt32(args[3]) == 0)
                _temporaryStats.Add(stats);
            else
                _predicatedStats.Add(stats);
            return true;
        }

        private bool CancelStat(object[] args) {
            ValidateArgs(args, typeof(string), typeof(bool));
            var stat = (Stat)Enum.Parse(typeof(Stat), Convert.ToString(args[0]));
            if (Convert.ToInt32(args[1]) == 0) {
                _temporaryStats.RemoveAll(predicate => predicate.Stat == stat);
                return true;
            }
            _predicatedStats.RemoveAll(predicate => predicate.Stat == stat);
            return true;
        }
        
        
        private float CalculatePercentStatChangers(Stat stat) {
            float totalValue = 1f;
            
            var temporary = _temporaryStats.SingleOrDefault(predicatedStat => predicatedStat.Stat == stat);
            totalValue += temporary?.PercentValue ?? 0;
            _temporaryStats.Remove(temporary);
            
            var predicate = _predicatedStats.SingleOrDefault(predicated => predicated.Stat == stat);
            totalValue += predicate?.PercentValue ?? 0;
            return totalValue;
        }

        private float CalculateFlatStatChangers(Stat stat) {
            float totalValue = 1f;
            
            var temporary = _temporaryStats.SingleOrDefault(predicatedStat => predicatedStat.Stat == stat);
            totalValue += temporary?.FlatValue ?? 0;
            _temporaryStats.Remove(temporary);
            
            var predicate = _predicatedStats.SingleOrDefault(predicated => predicated.Stat == stat);
            totalValue += predicate?.FlatValue ?? 0;
            return totalValue;
        }
        
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

        private sealed class PredicatedStats {
            public Stat Stat;
            public float FlatValue;
            public float PercentValue;
        }
    }
    
}
