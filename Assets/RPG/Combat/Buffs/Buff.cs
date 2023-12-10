using System;
using System.Collections.Generic;
using RPG.Combat.Modifiers.BaseTypes;
using RPG.Inventories.Items;
using RPG.Stats;
using RPG.Utils;
using UnityEngine;

namespace RPG.Combat.Buffs {
    public abstract class Buff : ScriptableObject, ISerializationCallbackReceiver {

        [SerializeField] private List<Modification> _modifications;
        
        private static Dictionary<string, Buff> _buffStore;

        protected float BuffDuration;
        protected bool IsBuffExpires;

        protected bool IsBuffStackable;
        protected int BuffMaxStacks;

        protected string Id;

        public float TotalDuration => BuffDuration;
        public bool IsExpiring => IsBuffExpires;
        public int MaxStacks => BuffMaxStacks;
        public bool IsStackable => IsBuffStackable;

        public string ID => Id;


        public void RegisterBuff(GameObject holder) {
            foreach (var modification in _modifications) {
                modification.RegisterModification(holder);
            }
        }

        public void UnRegisterBuff() {
            foreach (var modification in _modifications) {
                modification.UnregisterModification();
            }
        }

        public void SetStrength(int strength) {
            foreach (var modification in _modifications) {
                modification.SetStrength(strength);
            }
        }

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
