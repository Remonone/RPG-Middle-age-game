using RPG.Inventories.Items;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Inventories {
    public class InventoryItemIcon : MonoBehaviour {
        [SerializeField] private GameObject _container;
        
        public void SetItem(InventoryItem item) => SetItem(item, 0);
        
        public void SetItem(InventoryItem item, int count) {
            var iconImage = GetComponent<Image>();
            if (item == null) {
                iconImage.enabled = false;
            } else {
                iconImage.enabled = true;
                iconImage.sprite = item.Icon;
            }

            _container.SetActive(count > 1);
        }
    }
}
