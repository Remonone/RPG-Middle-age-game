using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat.Buffs {
    public class BuffContainer : MonoBehaviour {
        
        private List<BuffProperties> _buffList = new List<BuffProperties>();
        
        private void Update() {
            foreach (var prop in _buffList) {
                var buff = prop.Buff;
                if (buff.TotalDuration < Time.time - prop.CreationTime) {
                    ClearBuff(buff);
                    continue;
                }
                if(!buff.BuffTick.IsAvailable) continue;
                var strength = prop.Strength;
                buff.TickBuff(strength);
                buff.BuffTick.Reset();
            }
        }
        
        public void AddBuff(Buff buff) => AddBuff(buff, 1);

        public void AddBuff(Buff buff, int count) {
            if (count < 1) return;
            var prop = _buffList.SingleOrDefault(prop => prop.Buff == buff);
            if (prop != null && buff.IsStackable) {
                prop.Strength = Math.Max(buff.MaxStacks, prop.Strength + 1);
                ResetTimer(prop);
                return;
            }
            _buffList.Add(new BuffProperties {
                Buff = buff,
                CreationTime = Time.time,
                Strength = count
            });
        }

        public void RemoveBuff(Buff buff) => RemoveBuff(buff, 1);

        public void RemoveBuff(Buff buff, int quantity) {
            var prop = _buffList.SingleOrDefault(prop => prop.Buff == buff);
            if (prop == null) return;
            if (prop.Strength - quantity < 0) {
                _buffList.Remove(prop);
                return;
            }
            prop.Strength -= quantity;
        }

        public void ClearBuff(Buff buff) {
            var prop = _buffList.SingleOrDefault(prop => prop.Buff == buff);
            if (prop == null) return;
            _buffList.Remove(prop);
        }

        private void ResetTimer(BuffProperties buff) {
            buff.CreationTime = Time.time;
        }

        private sealed class BuffProperties {
            public Buff Buff;
            public float CreationTime;
            public int Strength;
        }
        
    }
}
