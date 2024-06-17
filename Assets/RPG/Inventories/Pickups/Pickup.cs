using System;
using RPG.Core.Cursors;
using RPG.Creatures.Player;
using RPG.Inventories.Items;
using Unity.Netcode;
using UnityEngine;

namespace RPG.Inventories.Pickups {
    public class Pickup : NetworkBehaviour, ITrajectory {

        [SerializeField] private Pickable _pickable = new();
        private bool _shouldBeDestroyed;

        public bool ShouldBeDestroyed => _shouldBeDestroyed;
        
        public event Action OnPickup;

        public void Setup(InventoryItem item, int count) {
            _pickable.StoredItem = item;
            _pickable.Count = count;
        }
        
        public Pickable PickUp() {
            Destroy(gameObject, .2f);
            _shouldBeDestroyed = true;
            OnPickup?.Invoke();
            return _pickable;
        }

        public bool TryCheckEquipmentSlot(EquipmentSlot slot) {
            var item = _pickable.StoredItem as EquipmentItem;
            if (item != null) {
                return item.Slot == slot;
            }

            return false;
        }

        public CursorType GetCursorType() {
            return CursorType.UI;
        }
        
        private void OnTriggerEnter(Collider other) {
            if (!IsServer) return;
            if (!other.CompareTag("Player")) return;
            if (!other.TryGetComponent<Inventory>(out var inventory)) return;
            var emptySlot = inventory.FindEmptySlot();
            if (emptySlot == -1) return;
            inventory.SetInventoryItemAtSlot(emptySlot, _pickable.StoredItem.ID, _pickable.Count);
            _shouldBeDestroyed = true;
            OnPickup?.Invoke();
            Destroy(gameObject);
        }
        
        public bool HandleRaycast(PlayerController invoker) {
            return !invoker.Map["Action"].WasPressedThisFrame();
        }
    }

    [Serializable]
    public class Pickable {
        public InventoryItem StoredItem;
        public int Count;
    }
}
