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
        [SerializeField] private Predicate _onEquipPredicate;
        [SerializeField] private Predicate _onUnequipPredicate;

        public EquipmentSlots Slot => _slot;
        public Predicate OnEquipPredicate => _onEquipPredicate;
        public Predicate OnUnequipPredicate => _onUnequipPredicate;
        
        public bool CanEquip(EquipmentSlots location) {
            return _slot == location;
        }

        [Serializable]
        public sealed class Predicate {
            public string CodePredicate;
            public string[] ComponentName;
        }
    }
}
