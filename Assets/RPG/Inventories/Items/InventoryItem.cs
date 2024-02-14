using System.Collections.Generic;
using RPG.Inventories.Pickups;
using RPG.Utils;
using UnityEngine;

namespace RPG.Inventories.Items {
    
    public abstract class InventoryItem : ScriptableObject, ISerializationCallbackReceiver {
        [SerializeField] private string _itemID;
        [SerializeField] private string _itemName;
        [SerializeField] [TextArea] private string _itemDescription;
        [SerializeField] private Sprite _itemIcon;
        [SerializeField] private bool _stackable;
        [SerializeField] private Pickup _pickup;

        private static Dictionary<string, InventoryItem> _itemStore;

        public string ID => _itemID;
        public string Name => _itemName;
        public string Description => _itemDescription;
        public Sprite Icon => _itemIcon;
        public bool IsStackable => _stackable;
        
        
        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            if (string.IsNullOrWhiteSpace(_itemID)) {
                _itemID = System.Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() { }

        public virtual string GetDescription() {
            return _itemDescription;
        }

        public Pickup SpawnPickup(Vector3 location, int count, Vector3 velocity) {
            var pickup = Instantiate(_pickup, location, Quaternion.identity);
            pickup.Setup(this, count);
            var rigidBody = pickup.GetComponent<Rigidbody>();
            rigidBody.velocity = velocity;
            return pickup;
        }

        public static InventoryItem GetItemByGuid(string itemID) {
            if (_itemStore == null) {
                _itemStore = new Dictionary<string, InventoryItem>();
                LoadStore();
            }
            if (itemID == null || !_itemStore.ContainsKey(itemID)) return null;
            return _itemStore[itemID];
        }

        private static void LoadStore() {
            var itemList = Resources.LoadAll<InventoryItem>(PropertyConstants.ITEMS_PATH);
            foreach (var item in itemList) {
                if (_itemStore.ContainsKey(item._itemID)) {
                    Debug.LogError($"There's a duplicate for objects: {_itemStore[item._itemID]} and {item}");
                    continue;
                }
                _itemStore[item._itemID] = item;
            }
        }
    }
}
