using System;
using System.Collections.Generic;
using RPG.Combat.Buffs;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories.Items {
    [CreateAssetMenu(fileName = "Equipment Item", menuName = "GumuPeachu/Inventory/Create new Equipment")]
    public class EquipmentItem : InventoryItem {

        [SerializeField] private EquipmentSlots _slot;
        [SerializeField] private List<Buff> _buffs;
        [SerializeField] private string _onEquipPredicate;
        [SerializeField] private string _onUnequipPredicate;

        public EquipmentSlots Slot => _slot;
        public string OnEquipPredicate => _onEquipPredicate;
        public string OnUnequipPredicate => _onUnequipPredicate;
        
        public bool CanEquip(EquipmentSlots location) {
            return _slot == location;
        }
    }
}
