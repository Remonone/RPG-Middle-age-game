using System;
using RPG.Inventories.Items;
using UnityEngine;

namespace RPG.Inventories.Pickups {
    public class Pickup : MonoBehaviour {

        private readonly Pickable _pickable = new();

        public void Setup(InventoryItem item, int count) {
            _pickable.StoredItem = item;
            _pickable.Count = count;
        }
        // TODO: Make able to pickup it through PlayerController class
        public Pickable PickUp() {
            Destroy(this, .1f);
            return _pickable;
        }
        
    }

    [Serializable]
    public class Pickable {
        public InventoryItem StoredItem;
        public int Count;
    }
}
