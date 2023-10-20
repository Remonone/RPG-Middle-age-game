using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Combat.Modifiers;
using RPG.Inventories.Items;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories {
    public class Equipment : MonoBehaviour, IStatModifier {

        private Dictionary<EquipmentSlots, EquipmentItem> _items = new Dictionary<EquipmentSlots, EquipmentItem>();

        public event Action OnEquipmentChange;
        
        public EquipmentItem GetEquipmentItem(EquipmentSlots equipmentSlot) {
            if(_items.ContainsKey(equipmentSlot)) return _items[equipmentSlot];
            return null;
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
        public float ReflectFlatStat(Stat stat) {
            return _items.Values.Where(item => item != null).Sum(item => item.ReflectFlatStat(stat));
        }
        public float ReflectPercentStat(Stat stat) {
            return _items.Values.Where(item => item != null).Sum(item => item.ReflectPercentStat(stat));
        }
    }
}
