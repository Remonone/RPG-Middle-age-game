using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Combat.Buffs;
using RPG.Combat.Modifiers;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories.Items {
    [CreateAssetMenu(fileName = "Equipment Item", menuName = "GumuPeachu/Inventory/Create new Equipment")]
    public class EquipmentItem : InventoryItem, IStatModifier {

        [SerializeField] private EquipmentSlots _slot;
        [SerializeField] private List<StatReflection> _statModifier;
        [SerializeField] private List<Buff> _buffs;

        public EquipmentSlots Slot => _slot;

        public float ReflectFlatStat(Stat stat) {
            var reflectionStat = _statModifier.SingleOrDefault(reflection => reflection.Stat == stat);
            if (reflectionStat == null) return 0;
            return reflectionStat.FlatReflection;
        }
        public float ReflectPercentStat(Stat stat) {
            var reflectionStat = _statModifier.SingleOrDefault(reflection => reflection.Stat == stat);
            if (reflectionStat == null) return 0;
            return reflectionStat.PercentReflection;
        }
        
        public bool CanEquip(EquipmentSlots location) {
            return _slot == location;
        }

        [Serializable]
        internal class StatReflection {
            public Stat Stat;
            public float FlatReflection;
            public float PercentReflection;
        }
    }
}
