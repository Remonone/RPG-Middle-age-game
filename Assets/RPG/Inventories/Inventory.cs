using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories {
    public class Inventory : MonoBehaviour {
        [SerializeField] private int _slotsCount;

        private InventorySlot[] _inventorySlots;

        private void Awake() {
            _inventorySlots = new InventorySlot[_slotsCount];
        }

        public void AddToInventorySlot(int slot, InventoryItem item, int count) {
            if (_inventorySlots[slot] != null) {
                AddToFirstEmptySlot(item, count);
                return;
            }
            if (!IsItemStackSatisfied(item, count)) {
                _inventorySlots[slot] = new InventorySlot { Item = item, Count = 1 };
                for (int i = 0; i < count - 1; i++) {
                    AddToFirstEmptySlot(item, 1);
                }
                return;
            }

            _inventorySlots[slot] = new InventorySlot { Item = item, Count = count };
        }
        private static bool IsItemStackSatisfied(InventoryItem item, int count) =>  item.IsStackable && count > 1;

        public void AddToFirstEmptySlot(InventoryItem item, int count) {
            int slotIndex = FindEmptySlot();
            if (slotIndex == -1) return; // TODO: Drop a Pickup;
            if (IsItemStackSatisfied(item, count)) {
                _inventorySlots[slotIndex] = new InventorySlot { Item = item, Count = count };
                return;
            }
            // TODO: Drop a Pickup;
        }
        private int FindEmptySlot() {
            for (int i = 0; i < _slotsCount; i++) {
                if (_inventorySlots[i].Item == null) return i;
            }

            return -1;
        }

        private sealed class InventorySlot {
            public InventoryItem Item;
            public int Count;
        }
    }
}
