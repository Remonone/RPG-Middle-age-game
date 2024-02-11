using RPG.Creatures.AI.Core;
using RPG.Inventories;
using RPG.Inventories.Items;
using RPG.Inventories.Pickups;
using UnityEngine;

namespace RPG.Creatures.AI.Actions {
    public class GetWeapon : GoapAction {

        [SerializeField] private float _searchRadius;
        [SerializeField] private Equipment _equipment;
        [SerializeField] private LayerMask _pickupLayer;
        
        public GetWeapon() {
            _prerequisites.Add(new StateObject {Name = "is_armed", Value = false});
            
            _effects.Add(new StateObject {Name = "is_armed", Value = true});
        }

        public override bool PerformAction(GameObject agent) {
            var pickup = Target.GetComponent<Pickup>();
            if (pickup == null) return false;
            var item = pickup.PickUp();
            var equipment = item.StoredItem as EquipmentItem;
            if (equipment == null) return false;
            _equipment.PlaceEquipment(equipment, EquipmentSlot.WEAPON);
            return true;
        }
        
        public override void DoReset() {
            Target = null;
        }
        
        public override bool IsDone() {
            return _equipment.GetEquipmentItem(EquipmentSlot.WEAPON) != null;
        }
        
        public override bool CheckProceduralPrerequisites(GameObject agent) {
            var agentPosition = agent.transform.position;
            var pickups = Physics.OverlapSphere(agentPosition, _searchRadius, _pickupLayer);
            if (pickups.Length <= 0) return false;
            var distance = float.MaxValue;
            foreach (var obj in pickups) {
                if(!obj.TryGetComponent<Pickup>(out var pickup)) continue;
                if(!pickup.TryCheckEquipmentSlot(EquipmentSlot.WEAPON)) continue;
                var localDistance = (pickup.transform.position - transform.position).magnitude;
                if (localDistance > distance) continue;
                Target = pickup.gameObject;
                distance = localDistance;
            }
            print(Target);
            return Target != null;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _searchRadius);
        }

        public override bool RequiresInRange() {
            return true;
        }
    }
}
