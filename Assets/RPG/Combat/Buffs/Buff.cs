using System;
using System.Collections.Generic;
using RPG.Combat.Modifiers;
using RPG.Inventories.Items;
using RPG.Stats;
using RPG.Utils;
using UnityEngine;

namespace RPG.Combat.Buffs {
    public abstract class Buff : ScriptableObject, IStatModifier, ISerializationCallbackReceiver {


        private static Dictionary<string, Buff> _buffStore;

        // TODO: Change condition of canceling from 2 cases to any cases
        protected float BuffDuration;
        protected bool IsBuffExpires;

        protected bool IsBuffStackable;
        protected int BuffMaxStacks;

        protected string Id;
        
        protected Cooldown TickToReset = new Cooldown {
            SetTimeToReset = 1f
        };

        public float TotalDuration => BuffDuration;
        public bool IsExpiring => IsBuffExpires;
        public int MaxStacks => BuffMaxStacks;
        public bool IsStackable => IsBuffStackable;

        public string ID => Id;

        public Cooldown BuffTick => TickToReset;
        

        /// <summary>
        /// Method invokes each second to handle effects from buff
        /// </summary>
        public abstract void TickBuff(int quantity);
        /// <summary>
        /// Method which cast when entity interact with another entity
        /// </summary>
        public abstract void CastBuff(int quantity, params object[] cast);

        public abstract float ReflectFlatStat(Stat stat);
        public abstract float ReflectPercentStat(Stat stat);

        public static Buff GetBuffById(string buffID) {
            if (_buffStore == null) {
                _buffStore = new Dictionary<string, Buff>();
                LoadStore();
            }
            if (buffID == null || !_buffStore.ContainsKey(buffID)) return null;
            return _buffStore[buffID];
        }
        

        private static void LoadStore() {
            var buffList = Resources.LoadAll<Buff>("Items");
            foreach (var buff in buffList) {
                if (_buffStore.ContainsKey(buff.ID)) {
                    Debug.LogError(string.Format("There's a duplicate for objects: {0} and {1}", _buffStore[buff.ID], buff));
                    continue;
                }
                _buffStore[buff.ID] = buff;
            }
        }

        public void OnBeforeSerialize() {
            if (string.IsNullOrWhiteSpace(ID)) {
                Id = Guid.NewGuid().ToString();
            }
        }
        public void OnAfterDeserialize() { }
    }
}
