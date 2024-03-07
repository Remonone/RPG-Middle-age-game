using System;
using RPG.Creatures.Player;
using RPG.Inventories.Items;
using RPG.UI.Cursors;
using UnityEngine;

namespace RPG.Inventories.Pickups {
    public class Pickup : MonoBehaviour, ITrajectory {

        private readonly Pickable _pickable = new();
        private bool _shouldBeDestroyed;

        public bool ShouldBeDestroyed => _shouldBeDestroyed;
        
        public event Action OnPickup;

        private void Awake() {
            _pickable.StoredItem = InventoryItem.GetItemByGuid("77973f65-3fd7-4404-a03c-fa46a7c72360");
            _pickable.Count = 1;
        }

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
            if (!other.CompareTag("Player")) return;
            if (!other.TryGetComponent<Inventory>(out var inventory)) return;
            inventory.AddToFirstEmptySlot(_pickable.StoredItem, _pickable.Count);
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
