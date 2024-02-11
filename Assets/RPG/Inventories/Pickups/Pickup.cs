using System;
using RPG.Inventories.Items;
using UnityEngine;

namespace RPG.Inventories.Pickups {
    public class Pickup : MonoBehaviour {

        private readonly Pickable _pickable = new();

        private void Awake() {
            _pickable.StoredItem = InventoryItem.GetItemByGuid("77973f65-3fd7-4404-a03c-fa46a7c72360");
            _pickable.Count = 1;
        }

        public void Setup(InventoryItem item, int count) {
            _pickable.StoredItem = item;
            _pickable.Count = count;
        }
        // TODO: Make able to pickup it through PlayerController class
        public Pickable PickUp() {
            Destroy(gameObject, .2f);
            return _pickable;
        }

        public bool TryCheckEquipmentSlot(EquipmentSlot slot) {
            var item = _pickable.StoredItem as EquipmentItem;
            if (item != null) {
                return item.Slot == slot;
            }

            return false;
        }

    }

    [Serializable]
    public class Pickable {
        public InventoryItem StoredItem;
        public int Count;
    }
}
