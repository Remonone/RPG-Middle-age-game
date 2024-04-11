using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RPG.Combat;
using RPG.Combat.Buffs;
using RPG.Core.Predicate.Interfaces;
using RPG.Saving;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace RPG.Stats {
    [RequireComponent(typeof(BuffContainer))]
    public class BaseStats : NetworkBehaviour, ISaveable, IPredicateHandler {
        [SerializeField] private StatsContainer _stats;

        [SerializeField] private NetworkVariable<int> _level = new();
        [SerializeField] private NetworkVariable<float> _experience = new();

        public CreatureClass CreatureClass => _stats.CreatureClass;
        public int Level => _level.Value;
        public float Experience => _experience.Value;
        
        public event Action OnLevelUp;
        public event Action OnStatUpdated;
        
        //TODO: CHANGE IT TO MODIFICATIONS
        private List<PredicatedStats> _predicatedStats = new();
        private List<PredicatedStats> _temporaryStats = new();

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            if (IsServer) {
                _level.Value = 1;
                _experience.Value = 0;
            }
        }

        public void AddExperience(float amount) {
            var exp = Stat.EXPERIENCE_TO_PROMOTE;
            _experience.Value += amount;
            if (_experience.Value > _stats.GetBaseStat(exp) + _stats.GetLevelStat(exp, _level.Value)) {
                _experience.Value = 0F;
                _level.Value += 1;
                OnLevelUp?.Invoke();
            }
            AddExperienceServerRpc(amount);
        }
        
        [ServerRpc]
        private void AddExperienceServerRpc(float amount) {
            var exp = Stat.EXPERIENCE_TO_PROMOTE;
            _experience.Value += amount;
            if (_experience.Value > _stats.GetBaseStat(exp) + _stats.GetLevelStat(exp, _level.Value)) {
                _experience.Value = 0F;
                _level.Value += 1;
                OnLevelUp?.Invoke();
            }
        }

        public float GetBaseStat(Stat stat) => _stats.GetBaseStat(stat);

        public float GetStatValue(Stat stat) {
            return (_stats.GetBaseStat(stat) + _stats.GetLevelStat(stat, _level.Value) + CalculateFlatStatChangers(stat)) * CalculatePercentStatChangers(stat);
        }
        
        public object Predicate(string command, object[] arguments) {
            object result = command switch {
                "AmplifyStat" => AmplifyStat(arguments),
                "CancelStat" => CancelStat(arguments),
                _ => null
            };
            if (result != null) OnStatUpdated?.Invoke();
            return result;
        }
        
        private bool AmplifyStat(object[] args) {
            var stats = new PredicatedStats {
                Stat = (Stat)Enum.Parse(typeof(Stat), Convert.ToString(args[0])),
                FlatValue = (float)Convert.ToDouble(args[1]),
                PercentValue = (float)Convert.ToDouble(args[2])
            };
            if(Convert.ToInt32(args[3]) == 0)
                _temporaryStats.Add(stats);
            else
                _predicatedStats.Add(stats);
            AmplifyStatServerRpc(stats, Convert.ToInt32(args[3]) == 0);
            return true;
        }
        
        [ServerRpc]
        private void AmplifyStatServerRpc(PredicatedStats predicatedStats, bool isTemporary) {
            if(isTemporary)
                _temporaryStats.Add(predicatedStats);
            else
                _predicatedStats.Add(predicatedStats);
        }

        private bool CancelStat(object[] args) {
            var stat = (Stat)Enum.Parse(typeof(Stat), Convert.ToString(args[0]));
            if (Convert.ToInt32(args[1]) == 0) {
                _temporaryStats.RemoveAll(predicate => predicate.Stat == stat);
            } else {
                _predicatedStats.RemoveAll(predicate => predicate.Stat == stat);
            }

            CancelStatServerRpc(Convert.ToString(args[0]), Convert.ToInt32(args[1]) == 0);
            return true;
        }
        
        [ServerRpc]
        private void CancelStatServerRpc(FixedString128Bytes statName, bool isTemporary) {
            var stat = (Stat)Enum.Parse(typeof(Stat), statName.Value);
            if (isTemporary) {
                _temporaryStats.RemoveAll(predicate => predicate.Stat == stat);
            } else {
                _predicatedStats.RemoveAll(predicate => predicate.Stat == stat);
            }
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
                new JProperty("level", _level.Value),
                new JProperty("experience", _experience.Value)
            );
        }
        public void RestoreFromJToken(JToken state) {
            _level.Value = (int)state["level"];
            _experience.Value = (int)state["experience"];
        }

        private sealed class PredicatedStats : INetworkSerializable {
            public Stat Stat;
            public float FlatValue;
            public float PercentValue;
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
                serializer.SerializeValue(ref Stat);
                serializer.SerializeValue(ref FlatValue);
                serializer.SerializeValue(ref PercentValue);
            }
        }
    }
    
}
