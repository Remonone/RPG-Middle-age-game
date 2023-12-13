using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RPG.Core.Predicate;
using RPG.Inventories.Items;
using RPG.Saving;
using RPG.Visuals.Display;
using UnityEngine;

namespace RPG.Inventories {
    public class Equipment : PredicateMonoBehaviour, ISaveable {

        private readonly Dictionary<EquipmentSlot, EquipmentItem> _items = new();
        private static readonly Dictionary<EquipmentSlot, GameObject> _positions = new();
        public event Action OnEquipmentChange;

        private void Start() {
            var positions = gameObject.GetComponentsInChildren<EquipmentVisualFlag>();
            foreach(var position in positions) _positions.Add(position.Slot, position.gameObject);
        }
        
        public EquipmentItem GetEquipmentItem(EquipmentSlot equipmentSlot) {
            return _items.ContainsKey(equipmentSlot) ? _items[equipmentSlot] : null;
        }
        
        public void PlaceEquipment(EquipmentItem item, EquipmentSlot equipmentSlot) {
            if (item.Slot != equipmentSlot) return;
            _items[equipmentSlot] = item;
            var predicate = string.Format(item.OnEquipPredicate.CodePredicate, 
                item.OnEquipPredicate.ComponentName.Select(component => ((PredicateMonoBehaviour)GetComponent(component)).ComponentID));
            PredicateWorker.ParsePredicate(predicate, ComponentID);
            item.RegisterModifications(gameObject);
            DisplayItem(item);
            OnEquipmentChange?.Invoke();
        }

        public void RemoveEquipment(EquipmentSlot equipmentSlot) {
            var predicate = 
                string.Format(_items[equipmentSlot].OnUnequipPredicate.CodePredicate, 
                    _items[equipmentSlot].OnUnequipPredicate.ComponentName.Select(component => ((PredicateMonoBehaviour)GetComponent(component)).ComponentID));
            PredicateWorker.ParsePredicate(predicate, ComponentID);
            _items[equipmentSlot].UnregisterModifications();
            _items[equipmentSlot] = null;
            DisplayItem(null);
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
                var slot = Enum.Parse<EquipmentSlot>((string)state["slot"]);
                _items[slot] = (EquipmentItem) item;
            }
        }
        public override void Predicate(string command, object[] arguments, out object result) {
            result = command switch {
                _ => null
            };
        }
        
        private void DisplayItem(EquipmentItem item) {
            if (item == null) {
                foreach(Transform children in _positions[item.Slot].transform) Destroy(children);
                return;
            }
            Instantiate(item.ItemModel, _positions[item.Slot].transform);
        }
    }
}
