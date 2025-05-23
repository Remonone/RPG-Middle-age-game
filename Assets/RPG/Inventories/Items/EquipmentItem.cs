﻿using System;
using System.Collections.Generic;
using RPG.Combat.DamageDefinition;
using RPG.Combat.Modifiers.BaseTypes;
using TMPro;
using UnityEngine;

namespace RPG.Inventories.Items {
    [CreateAssetMenu(fileName = "Equipment Item", menuName = "GumuPeachu/Inventory/Create new Equipment")]
    public class EquipmentItem : InventoryItem {

        [SerializeField] private EquipmentSlot _slot;
        [SerializeField] private DamageType _type;
        [SerializeField] private Predicate _onEquipPredicate;
        [SerializeField] private Predicate _onUnequipPredicate;
        [SerializeField] private List<Modification> _modifications;
        [SerializeField] private TextMeshProUGUI _descriptionPrefab;
        [SerializeField] private EquipmentModel _itemModel;
        [SerializeField] private AnimatorOverrideController _controller;
        
        public EquipmentSlot Slot => _slot;
        public DamageType Type => _type;
        public Predicate OnEquipPredicate => _onEquipPredicate;
        public Predicate OnUnequipPredicate => _onUnequipPredicate;
        public EquipmentModel ItemModel => _itemModel;
        public AnimatorOverrideController Controller => _controller;
        
        public bool CanEquip(EquipmentSlot location) {
            return _slot == location;
        }
        
        public void RegisterModifications(GameObject invoker) {
            foreach (var mod in _modifications) {
                mod.RegisterModification(invoker);
            }

            if (ReferenceEquals(_descriptionPrefab, null))return;
            var text = Instantiate(_descriptionPrefab, FindObjectOfType<Canvas>().transform);
            text.text = GetDescription();
        }

        public override string GetDescription() {
            var generalDescription = base.GetDescription();
            generalDescription += "\n";
            foreach (var modification in _modifications) {
                generalDescription += $"\n<b>{modification.ModificationName}</b>";
                generalDescription += $"\n{modification.ModificationDescription}";
            }
            return generalDescription;
        }

        public void UnregisterModifications() {
            foreach (var mod in _modifications) {
                mod.UnregisterModification();
            }
        }

        [Serializable]
        public sealed class Predicate {
            public string CodePredicate;
            public string ComponentName;
        }
    }
}
