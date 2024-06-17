using System.Linq;
using RPG.Inventories;
using RPG.Inventories.Items;
using UnityEngine;

namespace RPG.Quests.Objectives {
    public class CollectItemsObjective : QuestObjective {
        [SerializeField] private InventoryItem _itemToCollect;
        [SerializeField] private int _itemCountToCollect;

        private Inventory _ownerInventory;
        
        public override void Init(GameObject owner) {
            _ownerInventory = owner.GetComponent<Inventory>();
            _ownerInventory.OnInventoryUpdate += CheckOnItem;
        }
        
        private void OnDisable() {
            _ownerInventory.OnInventoryUpdate -= CheckOnItem;
        }

        private void CheckOnItem() {
            var stack = _ownerInventory.FindSlots(_itemToCollect).ToList().Select(item => item.Count).Sum();
            if (stack < _itemCountToCollect) return;
            CompleteObjective();
        }
    }
}
