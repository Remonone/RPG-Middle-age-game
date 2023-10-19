using RPG.Inventories;
using RPG.Inventories.Items;
using RPG.Utils.UI;
using UnityEngine;

namespace RPG.UI.Inventories.Slots {
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem> {
        [SerializeField] private InventoryItemIcon _imageUI;

        private int _index;
        private Inventory _inventory;

        public void Setup(Inventory inventory, int i) {
            var slot = inventory.GetItemInSlot(i);
            _index = i;
            _inventory = inventory;
            if (slot.Item == null) {
                _imageUI.SetItem(null);
                return;
            }
            _imageUI.SetItem(slot.Item);
        }
        public int MaxAcceptable(InventoryItem item) => item == null ? -1 : !item.IsStackable ? 1 : int.MaxValue;

        public void AddItems(InventoryItem item, int count) {
            _inventory.AddToInventorySlot(_index, item, count);
        }

        public InventoryItem GetItem() {
            return _inventory.GetItemInSlot(_index).Item;
        }

        public int GetNumber() {
            return _inventory.GetItemInSlot(_index).Count;
        }
        public void RemoveItems(int count) {
            _inventory.RemoveCountFromSlot(_index, count);
        }
    }
}
