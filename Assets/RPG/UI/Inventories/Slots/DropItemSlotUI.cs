using RPG.Inventories;
using RPG.Inventories.Items;
using RPG.Utils.UI;
using UnityEngine;

namespace RPG.UI.Inventories.Slots {
    public class DropItemSlotUI : MonoBehaviour, IDragDestination<InventoryItem> {

        [SerializeField] private GameObject _player;
        
        public int MaxAcceptable(InventoryItem item) {
            return int.MaxValue;
        }
        public void AddItems(InventoryItem item, int number) {
            _player.GetComponent<ItemDropper>().DropItem(item, number);
        }
    }
}
