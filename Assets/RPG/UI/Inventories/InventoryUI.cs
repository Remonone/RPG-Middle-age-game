using RPG.Inventories;
using RPG.UI.Inventories.Slots;
using UnityEngine;

namespace RPG.UI.Inventories {
    public class InventoryUI : MonoBehaviour {

        [SerializeField] private InventorySlotUI _slotUI;
        [SerializeField] private Inventory _inventory;

        private void Start() {
            _inventory.OnInventoryUpdate += RedrawUI;
            RedrawUI();
        }

        private void OnDestroy() {
            _inventory.OnInventoryUpdate -= RedrawUI;
        }
        private void RedrawUI() {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < _inventory.Size; i++) {
                var slot = Instantiate(_slotUI, transform);
                slot.Setup(_inventory, i);
                slot.name = "Inventory Slot " + i;
            }
        }

    }
}
