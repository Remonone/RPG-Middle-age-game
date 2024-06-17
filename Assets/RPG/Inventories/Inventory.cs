using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RPG.Inventories.Items;
using RPG.Inventories.Pickups;
using RPG.Saving;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace RPG.Inventories {
    public class Inventory : NetworkBehaviour, ISaveable {
        [SerializeField] private int _slotsCount;
        [SerializeField] private Pickup _pickup;
        
        private NetworkList<InventorySlot> _inventorySlots;

        public int Size => _inventorySlots.Count;

        public event Action OnInventoryUpdate;

        private void Awake() {
            _inventorySlots = new();
        }

        private void Start() {
            for (int i = 0; i < 64; i++) {
                AddInventoryItemAtSlotServerRpc("", 0);
            }
        }

        public void AddToInventorySlot(int slot, InventoryItem item, int count) {
            if (_inventorySlots[slot].ItemId.Value != "" && _inventorySlots[slot].ItemId.Value != item.ID) {
                AddToFirstEmptySlot(item, count);
                return;
            }
            SetInventoryItemAtSlotServerRpc(slot, item.ID, count + _inventorySlots[slot].Count);
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
            SetInventoryItemAtSlotServerRpc(slotIndex, item.ID, count);
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
            if (_inventorySlots[slot].ItemId.Value == "") return false;
            var finalCount = _inventorySlots[slot].Count - count;
            if (_inventorySlots[slot].Count - count > 0) {
                SetInventoryItemAtSlotServerRpc(slot, _inventorySlots[slot].ItemId, finalCount);
                return true;
            }
            SetInventoryItemAtSlotServerRpc(slot, "", 0);
            
            return finalCount == 0;
        }

        public void DropItem(int slot, int count) {
            if (_inventorySlots[slot].ItemId.Value == "") return;
            int itemActual = _inventorySlots[slot].Count;
            string item = _inventorySlots[slot].ItemId.Value;
            if (RemoveCountFromSlot(slot, count)) {
                SpawnPickupServerRpc(item, count);
                return;
            }
            SpawnPickupServerRpc(item, itemActual);
        }
        
        [ServerRpc]
        private void SpawnPickupServerRpc(FixedString128Bytes itemId, int count) {
            var pickup = Instantiate(_pickup, transform.forward, Quaternion.identity);
            var item = InventoryItem.GetItemByGuid(itemId.Value);
            pickup.Setup(item, count);
        }

        public InventorySlot GetItemInSlot(int slot) => _inventorySlots[slot];

        public int FindStack(InventoryItem item) {
            for (int i = 0; i < _inventorySlots.Count; i++) {
                if (_inventorySlots[i].ItemId.Value == item.ID) return i;
            }

            return -1;
        }

        public IEnumerable<InventorySlot> FindSlots(InventoryItem item) {
            foreach (var slot in _inventorySlots) {
                if (slot.ItemId.Value == item.ID) yield return slot;
            }
        }

        
        public int FindEmptySlot() {
            for (int i = 0; i < _slotsCount; i++) {
                if (_inventorySlots[i].ItemId.Value == "") return i;
            }
            return -1;
        }
        
        private static bool IsItemStackSatisfied(InventoryItem item, int count) =>  item.IsStackable && count > 1;
        
        public struct InventorySlot : IEquatable<InventorySlot>, INetworkSerializable {
            public FixedString128Bytes ItemId;
            public int Count;

            public bool Equals(InventorySlot other) {
                return ItemId.Equals(other.ItemId) && Count == other.Count;
            }

            public override bool Equals(object obj) {
                return obj is InventorySlot other && Equals(other);
            }

            public override int GetHashCode() {
                return HashCode.Combine(ItemId, Count);
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
                serializer.SerializeValue(ref ItemId);
                serializer.SerializeValue(ref Count);
            }
        }

        public JToken CaptureAsJToken() {
            InventorySlot[] collection = new InventorySlot[16];
            for (int i = 0; i < 16; i++) {
                collection[i] = _inventorySlots[i];
            }
            var inventoryState = new JArray(
                        from slot in collection
                        select new JObject(
                            new JProperty("slot", Array.FindIndex(collection, pos => pos.ItemId.Value == slot.ItemId.Value)),
                            new JProperty("id", new JValue(slot.ItemId.Value)),
                            new JProperty("count", slot.Count)
                        )
                    );
            return inventoryState;
        }
        public void RestoreFromJToken(JToken state) {
            if (!IsServer) return;
            foreach (var slot in state) {
                var index = (int)slot["slot"];
                var count = (int)slot["count"];
                var itemId = (string)slot["id"];
                SetInventoryItemAtSlot(index, itemId, count);
            }
        }

        [ServerRpc]
        private void SetInventoryItemAtSlotServerRpc(int slot, FixedString128Bytes itemId, int count) {
            SetInventoryItemAtSlot(slot, itemId, count);
        }

        public void SetInventoryItemAtSlot(int slot, FixedString128Bytes itemId, int count) {
            if (!IsServer) return;
            InventorySlot item = new InventorySlot {
                Count = count,
                ItemId = itemId
            };
            _inventorySlots[slot] = item;
            ClientRpcParams param = new ClientRpcParams {
                Send = {
                    TargetClientIds = new[] { GetComponent<NetworkObject>().OwnerClientId }
                }
            };
            UpdateOwnerInventoryClientRpc(param);
        }

        [ClientRpc]
        private void UpdateOwnerInventoryClientRpc(ClientRpcParams clientRpcParams) {
            OnInventoryUpdate?.Invoke();
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddInventoryItemAtSlotServerRpc(FixedString128Bytes itemId, int count) {
            InventorySlot item = new InventorySlot {
                Count = count,
                ItemId = itemId
            };
            _inventorySlots.Add(item);
        }
    }
}
