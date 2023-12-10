using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat.Buffs {
    public class BuffContainer : MonoBehaviour, ISaveable {
        
        private List<BuffProperties> _buffList = new();
        
        private void Update() {
            foreach (var prop in _buffList) {
                var buff = prop.Buff;
                if (buff.TotalDuration < Time.time - prop.CreationTime) {
                    ClearBuff(buff);
                }
            }
        }
        
        public void AddBuff(Buff buff) => AddBuff(buff, 1);

        public void AddBuff(Buff buff, int strength) {
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

        public void RemoveBuff(Buff buff) => RemoveBuff(buff, 1);

        public void RemoveBuff(Buff buff, int quantity) {
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

        public void ClearBuff(Buff buff) {
            var prop = _buffList.SingleOrDefault(prop => prop.Buff == buff);
            if (prop == null) return;
            prop.Buff.UnRegisterBuff();
            _buffList.Remove(prop);
        }

        private void ResetTimer(BuffProperties buff) {
            buff.CreationTime = Time.time;
        }

        public JToken CaptureAsJToken() { // TODO: Make realization of this when will make Predicate system.
            return new JArray(
                    
                );
        }
        public void RestoreFromJToken(JToken state) { } // TODO: Make realization of this when will make Predicate system.
        
        private sealed class BuffProperties {
            public Buff Buff;
            public float CreationTime;
            public int Strength; // i.e. Count
        }
    }
}
