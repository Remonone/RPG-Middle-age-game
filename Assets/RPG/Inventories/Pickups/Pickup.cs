using System;
using RPG.Inventories.Items;
using UnityEngine;

namespace RPG.Inventories.Pickups {
    public class Pickup : MonoBehaviour {

        private Pickable _picable = new ();

        public void Setup(InventoryItem item, int count) {
            _picable.StoredItem = item;
            _picable.Count = count;
        }
        // TODO: Make able to pickup it through PlayerController class
        public Pickable PickUp() {
            Destroy(this, .1f);
            return _picable;
        }
        
    }

    [Serializable]
    public class Pickable {
        public InventoryItem StoredItem;
        public int Count;
    }
}
