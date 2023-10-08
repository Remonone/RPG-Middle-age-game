using UnityEngine;

namespace RPG.Inventories {
    
    public abstract class InventoryItem : ScriptableObject, ISerializationCallbackReceiver {
        [SerializeField] private string _itemID;
        [SerializeField] private string _itemName;
        [SerializeField] private string _itemDescription;
        [SerializeField] private Sprite _itemIcon;
        [SerializeField] private bool _stackable;

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

        public void DropPickup(int count) {
            
        }
    }
}
