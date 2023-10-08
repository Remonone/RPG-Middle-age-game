using System;
using RPG.Inventories;
using UnityEngine;

namespace RPG.UI.Inventories {
    public class InventoryUI : MonoBehaviour {

        [SerializeField] private InventorySlotUI _slotUI;

        private Inventory _inventory;

        private void Awake() {
            _inventory = Inventory.GetPlayerInventory();
            _inventory.OnInventoryUpdate += RedrawUI;
        }

        private void Start() {
            RedrawUI();
        }
        private void RedrawUI() {
            foreach (Transform child in transform) {
                Destroy(child);
            }
            for (int i = 0; i < _inventory.Size; i++) {
                var slot = Instantiate(_slotUI, transform);
                slot.name = "Inventory Slot " + i;
            }
        }

    }
}
