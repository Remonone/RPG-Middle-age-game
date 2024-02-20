using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RPG.Core.Predicate.Interfaces;
using RPG.Saving;
using UnityEngine;

namespace RPG.Combat.Buffs {
    public class BuffContainer : MonoBehaviour, ISaveable, IPredicateHandler {
        
        private List<BuffProperties> _buffList = new();
        
        private void Update() {
            foreach (var prop in _buffList) {
                var buff = prop.Buff;
                if (buff.TotalDuration < Time.time - prop.CreationTime) {
                    ClearBuff(buff);
                }
            }
        }

        private void AddBuff(Buff buff, int strength) {
            if (strength < 1) return;
            var prop = _buffList.SingleOrDefault(prop => prop.Buff == buff);
            if (prop != null && buff.IsStackable) {
                prop.Strength = Math.Max(buff.MaxStacks, prop.Strength + 1);
                prop.Buff.SetStrength(prop.Strength);
                ResetTimer(prop);
                return;
            }

            var buffProp = new BuffProperties {
                Buff = buff,
                CreationTime = Time.time,
                Strength = Math.Min(strength, buff.MaxStacks)
            };
            
            _buffList.Add(buffProp);
            buffProp.Buff.SetStrength(buffProp.Strength);
            buffProp.Buff.RegisterBuff(holder: gameObject);
        }

        private void RemoveBuff(Buff buff, int quantity) {
            var prop = _buffList.SingleOrDefault(prop => prop.Buff == buff);
            if (prop == null) return;
            if (prop.Strength - quantity < 0) {
                prop.Buff.UnRegisterBuff();
                _buffList.Remove(prop);
                return;
            }
            prop.Strength -= quantity;
            prop.Buff.SetStrength(prop.Strength);
        }

        private bool ClearBuff(Buff buff) {
            var prop = _buffList.SingleOrDefault(prop => prop.Buff == buff);
            if (prop == null) return false;
            prop.Buff.UnRegisterBuff();
            _buffList.Remove(prop);
            return true;
        }

        private void ResetTimer(BuffProperties buff) {
            buff.CreationTime = Time.time;
        }

        public JToken CaptureAsJToken() { 
            return new JArray(
                    from buff in _buffList
                    select new JObject(
                            new JProperty("id", buff.Buff.ID),
                            new JProperty("duration", Time.time - buff.CreationTime),
                            new JProperty("strength", buff.Strength)
                        )
                );
        }
        public void RestoreFromJToken(JToken state) {
            foreach (var buffContainer in state) {
                string buffID = (string)buffContainer["id"];
                float creationDelta = (float)buffContainer["duration"];
                int strength = (int)buffContainer["strength"];
                var buff = Buff.GetBuffById(buffID);
                BuffProperties properties = new BuffProperties {
                    Buff = buff,
                    CreationTime = Time.time - creationDelta,
                    Strength = strength
                };
                _buffList.Add(properties);
            }
        }
        
        private sealed class BuffProperties {
            public Buff Buff;
            public float CreationTime;
            public int Strength; // i.e. Count
        }

        public object Predicate(string command, object[] arguments) {
            return command switch {
                "AddBuff" => AddBuffToCreature(arguments),
                "ReduceBuff" => ReduceBuffOfCreature(arguments),
                "HasBuff" => IsBuffInList(arguments),
                "ClearBuff" => ClearBuffOfCreature(arguments),
                _ => null
            };
        }
        private object ClearBuffOfCreature(object[] arguments) {
            string buffID = Convert.ToString(arguments[0]);
            if (string.IsNullOrWhiteSpace(buffID)) return false;
            Buff buffToClear = Buff.GetBuffById(buffID);
            return ClearBuff(buffToClear);
        }
        
        private object IsBuffInList(object[] arguments) {
            string buffID = Convert.ToString(arguments[0]);
            if (string.IsNullOrWhiteSpace(buffID)) return false;
            return _buffList.Any(record => record.Buff.ID == buffID);
        }
        private object ReduceBuffOfCreature(object[] arguments) {
            string buffID = Convert.ToString(arguments[0]);
            if (string.IsNullOrWhiteSpace(buffID)) return false;
            string strength = Convert.ToString(arguments[1]);
            if (string.IsNullOrWhiteSpace(strength)) return false;
            int strengthValue = Int32.Parse(strength);
            Buff buff = Buff.GetBuffById(buffID);
            if (ReferenceEquals(buff, null)) return false;
            RemoveBuff(buff, strengthValue);
            return true;
        }
        
        private object AddBuffToCreature(object[] arguments) {
            string buffID = Convert.ToString(arguments[0]);
            if (string.IsNullOrWhiteSpace(buffID)) return false;
            string strength = Convert.ToString(arguments[1]);
            if (string.IsNullOrWhiteSpace(strength)) return false;
            int strengthValue = Int32.Parse(strength);
            Buff buff = Buff.GetBuffById(buffID);
            if (ReferenceEquals(buff, null)) return false;
            AddBuff(buff, strengthValue);
            return true;
        }
    }
}
