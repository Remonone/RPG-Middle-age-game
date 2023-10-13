using RPG.Inventories;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Inventories {
    public class InventorySlotUI : MonoBehaviour {
        [SerializeField] private Image _imageUI;

        public void Setup(Inventory inventory, int i) {
            var slot = inventory.GetItemInSlot(i);
            if (slot == null) return;
            _imageUI.sprite = slot.Item.Icon;
        }
    }
}
