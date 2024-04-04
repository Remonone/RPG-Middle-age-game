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
    public class Equipment : MonoBehaviour, ISaveable {
        [SerializeField] private Animator _animator;

        private PredicateMonoBehaviour _predicate;

        private readonly Dictionary<EquipmentSlot, EquipmentItem> _items = new();
        private readonly Dictionary<EquipmentSlot, GameObject> _positions = new();
        public event Action OnEquipmentChange;

        private void Awake() {
            _predicate = GetComponent<PredicateMonoBehaviour>();
        }

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
            ApplyPredicate(_items[equipmentSlot].OnEquipPredicate);
            item.RegisterModifications(gameObject);
            if(!ReferenceEquals(item.Controller, null))
                _animator.runtimeAnimatorController = item.Controller;
            DisplayItem(equipmentSlot, item);
            OnEquipmentChange?.Invoke();
        }

        public void RemoveEquipment(EquipmentSlot equipmentSlot) {
            ApplyPredicate(_items[equipmentSlot].OnUnequipPredicate);
            _items[equipmentSlot].UnregisterModifications();
            _items[equipmentSlot] = null;
            var controller = _animator.runtimeAnimatorController as AnimatorOverrideController;
            if (controller != null) _animator.runtimeAnimatorController = controller.runtimeAnimatorController;
            DisplayItem(equipmentSlot, null);
            OnEquipmentChange?.Invoke();
        }

        private void ApplyPredicate(EquipmentItem.Predicate predicate) {
            if (predicate.CodePredicate == "" || predicate.ComponentName == "") return;
            var formatted = 
                string.Format(predicate.CodePredicate, 
                    ((PredicateMonoBehaviour)GetComponent(predicate.ComponentName)).EntityID);
            PredicateWorker.ExecutePredicate(formatted, _predicate.EntityID, out _);
        }
        
        public JToken CaptureAsJToken() {
            var equipmentInfo = new JArray(
                from equipmentID in _items 
                select new JObject(
                        new JProperty("slot", equipmentID.Key.ToString()),
                        new JProperty("id", equipmentID.Value.ID)
                    )
            );
            return equipmentInfo;
        }
        public void RestoreFromJToken(JToken state) {
            foreach (var item in state) {
                if (item.Type == JTokenType.Null) continue;
                var equipment = InventoryItem.GetItemByGuid((string)item["id"]);
                var slot = Enum.Parse<EquipmentSlot>((string)item["slot"]);
                _items[slot] = (EquipmentItem) equipment;
            }
        }

        private void DisplayItem(EquipmentSlot slot, EquipmentItem item) {
            if (item == null || item.ItemModel == null) {
                foreach (Transform children in _positions[slot].transform) {
                    if(children.gameObject.TryGetComponent<EquipmentModel>(out _)) Destroy(children.gameObject);
                }
                return;
            }
            Instantiate(item.ItemModel, _positions[slot].transform);
        }
    }
}
