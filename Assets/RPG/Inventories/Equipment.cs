using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RPG.Core.Predicate;
using RPG.Inventories.Items;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories {
    public class Equipment : PredicateMonoBehaviour, ISaveable {

        private Dictionary<EquipmentSlots, EquipmentItem> _items = new Dictionary<EquipmentSlots, EquipmentItem>();

        private BaseStats _stats;

        public event Action OnEquipmentChange;
        
        public EquipmentItem GetEquipmentItem(EquipmentSlots equipmentSlot) {
            if(_items.ContainsKey(equipmentSlot)) return _items[equipmentSlot];
            return null;
        }
        public void PlaceEquipment(EquipmentItem item, EquipmentSlots equipmentSlot) {
            if (item.Slot != equipmentSlot) return;
            _items[equipmentSlot] = item;
            var predicate = string.Format(item.OnEquipPredicate, _stats.ComponentID);
            PredicateWorker.ParsePredicate(predicate, ComponentID);
            OnEquipmentChange?.Invoke();
        }
        public void RemoveEquipment(EquipmentSlots equipmentSlot) {
            var predicate = string.Format(_items[equipmentSlot].OnUnequipPredicate, _stats.ComponentID);
            PredicateWorker.ParsePredicate(predicate, ComponentID);
            _items[equipmentSlot] = null;
            OnEquipmentChange?.Invoke();
        }
        
        public JToken CaptureAsJToken() {
            var equipmentInfo = new JProperty("equipment", new JArray(
                from equipmentID in _items 
                select new JObject(
                        new JProperty("slot", equipmentID.Key.ToString()),
                        new JProperty("id", equipmentID.Value.ID)
                    )
            ));
            return equipmentInfo;
        }
        public void RestoreFromJToken(JToken state) {
            foreach (var id in state["equipment"]) {
                var item = InventoryItem.GetItemByGuid((string)id);
                var slot = Enum.Parse<EquipmentSlots>((string)state["slot"]);
                _items[slot] = (EquipmentItem) item;
            }
        }
        public override void Predicate(string command, object[] arguments, out object result) {
            result = command switch {
                _ => null
            };
        }
    }
}
