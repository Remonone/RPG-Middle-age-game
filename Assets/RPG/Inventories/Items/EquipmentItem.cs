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
        
        [SerializeField] private List<StatReflection> _statModifier;
        [SerializeField] private List<Buff> _buffs;
        

        public float ReflectFlatStat(Stat stat) {
            return _statModifier.Single(reflection => reflection.Stat == stat).FlatReflection;
        }
        public float ReflectPercentStat(Stat stat) {
            return _statModifier.Single(reflection => reflection.Stat == stat).PercentReflection;
        }

        [Serializable]
        internal class StatReflection {
            public Stat Stat;
            public float FlatReflection;
            public float PercentReflection;
        }
    }
}
