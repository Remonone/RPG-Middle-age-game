using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RPG.Inventories.Items;
using RPG.Inventories.Pickups;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventories {
    public class Inventory : MonoBehaviour, ISaveable {
        [SerializeField] private int _slotsCount;
        [SerializeField] private Pickup _pickup;

        private InventorySlot[] _inventorySlots;

        public int Size => _inventorySlots.Length;

        public event Action OnInventoryUpdate;

        private void Awake() {
            _inventorySlots = new InventorySlot[_slotsCount];
            for (int i = 0; i < Size; i++) {
                _inventorySlots[i] = new InventorySlot {
                    Item = null,
                    Count = 0
                };
            }
        }

        private void Start() {
            _inventorySlots[0] = new InventorySlot {
                Item = InventoryItem.GetItemByGuid("168820e5-f325-4e1e-9948-126e5ada4f18"),
                Count = 1
            };
            _inventorySlots[1] = new InventorySlot {
                Item = InventoryItem.GetItemByGuid("77973f65-3fd7-4404-a03c-fa46a7c72360"),
                Count = 1
            };
            OnInventoryUpdate?.Invoke();
        }

        public void AddToInventorySlot(int slot, InventoryItem item, int count) {
            if (_inventorySlots[slot].Item != null && _inventorySlots[slot].Item != item) {
                AddToFirstEmptySlot(item, count);
                return;
            }
            _inventorySlots[slot] = new InventorySlot { Item = item, Count = count };
            OnInventoryUpdate?.Invoke();
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
            if (_inventorySlots[slot].Count > 0) return true;
            var finalCount = _inventorySlots[slot].Count;
            _inventorySlots[slot] = new InventorySlot{ Item = null, Count = 0};
            OnInventoryUpdate?.Invoke();
            return finalCount == 0;
        }

        public void DropItem(int slot, int count) {
            if (_inventorySlots[slot] == null) return;
            int itemActual = _inventorySlots[slot].Count;
            InventoryItem item = _inventorySlots[slot].Item;
            if (RemoveCountFromSlot(slot, count)) {
                SpawnPickup(item, count);
                return;
            }
            SpawnPickup(item, itemActual);
        }
        
        private void SpawnPickup(InventoryItem item, int count) {
            var pickup = Instantiate(_pickup, transform.forward, Quaternion.identity);
            pickup.Setup(item, count);
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

        public JToken CaptureAsJToken() {
            var inventoryState = new JArray(
                            from item in _inventorySlots
                            select new JObject(
                                    new JProperty("slot", Array.FindIndex(_inventorySlots, slot => slot == item)),
                                    new JProperty("item" , new JObject(
                                                new JProperty("id", item.Item.ID),
                                                new JProperty("count", item.Count)
                                            ))
                                )
                        );
            return inventoryState;
        }
        public void RestoreFromJToken(JToken state) {
            foreach (var slot in state) {
                var index = (int)slot["slot"];
                var itemConfig = slot["item"];
                var item = InventoryItem.GetItemByGuid((string)itemConfig["id"]);
                var count = (int)itemConfig["count"];
                _inventorySlots[index] = new InventorySlot {
                    Item = item,
                    Count = count
                };
            }
        }
    }
}
