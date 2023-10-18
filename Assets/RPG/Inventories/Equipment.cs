using System;
using System.Collections.Generic;
using RPG.Inventories.Items;
using UnityEngine;

namespace RPG.Inventories {
    public class Equipment : MonoBehaviour {

        private Dictionary<EquipmentSlots, EquipmentItem> _items = new Dictionary<EquipmentSlots, EquipmentItem>();

        public event Action OnEquipmentChange;
        
        public EquipmentItem GetEquipmentItem(EquipmentSlots equipmentSlot) {
            return _items[equipmentSlot];
        }
        public void PlaceEquipment(EquipmentItem item, EquipmentSlots equipmentSlot) {
            if (item.Slot != equipmentSlot) return;
            _items[equipmentSlot] = item;
            OnEquipmentChange?.Invoke();
        }
        public void RemoveEquipment(EquipmentSlots equipmentSlot) {
            _items[equipmentSlot] = null;
            OnEquipmentChange?.Invoke();
        }
    }
}
