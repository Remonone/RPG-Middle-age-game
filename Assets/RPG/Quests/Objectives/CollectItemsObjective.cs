using RPG.Inventories;
using RPG.Inventories.Items;
using UnityEngine;

namespace RPG.Quests.Objectives {
    public class CollectItemsObjective : QuestObjective {
        [SerializeField] private InventoryItem _itemToCollect;
        [SerializeField] private int _itemCountToCollect;

        private void OnEnable() {
            GetComponent<Inventory>().OnInventoryUpdate += CheckOnItem;
        }

        private void OnDisable() {
            GetComponent<Inventory>().OnInventoryUpdate -= CheckOnItem;
        }

        public void CheckOnItem() {
            var stack = GetComponent<Inventory>().FindStack(_itemToCollect);
            if (stack < _itemCountToCollect) return;
            CompleteObjective();
        }
    }
}
