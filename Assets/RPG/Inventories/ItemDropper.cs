using System.Collections.Generic;
using RPG.Inventories.Items;
using RPG.Inventories.Pickups;
using UnityEngine;

namespace RPG.Inventories {
    public class ItemDropper : MonoBehaviour {
        [SerializeField] private float _dropSpreadStrength;
        [SerializeField] private Vector3 _spawnOffset;
        
        private List<Pickup> _droppedItems = new();

        public void DropItem(InventoryItem item, int count) {
            var pickup = item.SpawnPickup(transform.position + _spawnOffset, count, GetRandomVelocity());
            pickup.OnPickup += OnPickupDestroy;
            _droppedItems.Add(pickup);
        }
        
        private void OnPickupDestroy() {
            var pickupToDestroy = _droppedItems.Find(pickup => pickup.ShouldBeDestroyed);
            _droppedItems.Remove(pickupToDestroy);
        }
        
        private Vector3 GetRandomVelocity() {
            var velocity = Random.onUnitSphere * _dropSpreadStrength;
            velocity.y += 1f;
            return velocity;
        }

        public void DropItem(InventoryItem item) => DropItem(item, 1);

        
    }
}
