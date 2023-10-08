using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Inventories {
    public class Inventory : MonoBehaviour {
        [SerializeField] private int _slotsCount;

        private InventorySlot[] _inventorySlots;

        public event Action OnInventoryUpdate;

        public int Size => _inventorySlots.Length;

        private void Awake() {
            _inventorySlots = new InventorySlot[_slotsCount];
        }

        public void AddToInventorySlot(int slot, InventoryItem item, int count) {
            if (_inventorySlots[slot] != null && _inventorySlots[slot].Item != item) {
                AddToFirstEmptySlot(item, count);
                return;
            }
            OnInventoryUpdate?.Invoke();
            _inventorySlots[slot] = new InventorySlot { Item = item, Count = count };
        }

        public bool AddToFirstEmptySlot(InventoryItem item, int count) {
            int existingPosition = FindStack(item);
            if (existingPosition != -1) {
                AddToInventorySlot(existingPosition, item, count);
                return true;
            }
            int slotIndex = FindEmptySlot();
            if (slotIndex == -1) return false;
            _inventorySlots[slotIndex] = new InventorySlot { Item = item, Count = count };
            OnInventoryUpdate?.Invoke();
            return true;
        }

        public bool RemoveItemFromSlot(int slot) => RemoveCountFromSlot(slot, 1);
        
        /// <summary>
        ///     
        /// </summary>
        /// <param name="slot">Slot which to decrease</param>
        /// <param name="count">Count of items which require to clear</param>
        /// <returns>True if count is fully decreased(i.g. count = 4, item count = 5 -> return true), else False(i.g. count = 5, item count = 3 -> return false).</returns>
        public bool RemoveCountFromSlot(int slot, int count) {
            if (_inventorySlots[slot] == null) return false;
            _inventorySlots[slot].Count -= count;
            OnInventoryUpdate?.Invoke();
            if (_inventorySlots[slot].Count > 0) return true;
            var finalCount = _inventorySlots[slot].Count;
            _inventorySlots[slot] = null;
            return finalCount == 0;
        }

        public InventorySlot GetItemInSlot(int slot) => _inventorySlots[slot];

        public int FindStack(InventoryItem item) => Array.FindIndex(_inventorySlots, slot => slot.Item == item);

        public IEnumerable<InventorySlot> FindSlots(InventoryItem item) => _inventorySlots.Where(slot => slot.Item == item);

        public static Inventory GetPlayerInventory() {
            var player = GameObject.FindWithTag("Player");
            return player.GetComponent<Inventory>();
        }
        
        private int FindEmptySlot() {
            for (int i = 0; i < _slotsCount; i++) {
                if (_inventorySlots[i].Item == null) return i;
            }
            return -1;
        }
        
        private static bool IsItemStackSatisfied(InventoryItem item, int count) =>  item.IsStackable && count > 1;
        
        public sealed class InventorySlot {
            public InventoryItem Item;
            public int Count;
        }
    }
}
