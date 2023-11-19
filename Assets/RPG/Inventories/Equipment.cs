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
        
        // 1. Receive Component by tag
        // 2. Make PlayerController as PredicateMonoBehaviour
        // 3. Make function in PlayerController which work with environment
        // 4. Call PlayerController and receive id of the component which would be written in variable
        // 5. When you call component you may call id of the component by variable
        
        private Dictionary<EquipmentSlots, EquipmentItem> _items = new();
        
        public event Action OnEquipmentChange;

        
        public EquipmentItem GetEquipmentItem(EquipmentSlots equipmentSlot) {
            if(_items.ContainsKey(equipmentSlot)) return _items[equipmentSlot];
            return null;
        }
        public void PlaceEquipment(EquipmentItem item, EquipmentSlots equipmentSlot) {
            if (item.Slot != equipmentSlot) return;
            _items[equipmentSlot] = item;
            var predicate = string.Format(item.OnEquipPredicate.CodePredicate, 
                item.OnEquipPredicate.ComponentName.Select(component => ((PredicateMonoBehaviour)GetComponent(component)).ComponentID));
            PredicateWorker.ParsePredicate(predicate, ComponentID);
            item.RegisterAmplifiers(gameObject);
            OnEquipmentChange?.Invoke();
        }
        
        public void RemoveEquipment(EquipmentSlots equipmentSlot) {
            var predicate = 
                string.Format(_items[equipmentSlot].OnUnequipPredicate.CodePredicate, 
                    _items[equipmentSlot].OnUnequipPredicate.ComponentName.Select(component => ((PredicateMonoBehaviour)GetComponent(component)).ComponentID));
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
