using RPG.Inventories;
using RPG.Inventories.Items;
using RPG.Utils.UI;
using UnityEngine;

namespace RPG.UI.Inventories.Slots {
    public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem> {

        [SerializeField] private InventoryItemIcon _equipmentImage;
        [SerializeField] private EquipmentSlots _equipmentSlot;

        private Equipment _equipment;

        private void Awake() {
            var player = GameObject.FindWithTag("Player");
            _equipment = player.GetComponent<Equipment>();
            _equipment.OnEquipmentChange += RedrawSlot;
        }

        private void Start() {
            RedrawSlot();
        }
        
        private void RedrawSlot() {
            var item = _equipment.GetEquipmentItem(_equipmentSlot);
            if (item == null) {
                _equipmentImage.SetItem(null);
                return;
            }
            _equipmentImage.SetItem(item);
        }

        public int GetNumber() {
            return 1;
        }
        public void RemoveItems(int number) {
            _equipment.RemoveEquipment(_equipmentSlot);
        }
        public int MaxAcceptable(InventoryItem item) {
            EquipmentItem equipableItem = item as EquipmentItem;
            if (equipableItem == null) return 0;
            if (equipableItem.Slot != _equipmentSlot) return 0;
            if (GetItem() != null) return 0;

            return 1;
        }
        
        public void AddItems(InventoryItem item, int number) {
            _equipment.PlaceEquipment((EquipmentItem)item, _equipmentSlot);
        }
        public InventoryItem GetItem() {
            return _equipment.GetEquipmentItem(_equipmentSlot);
        }
    }
}
