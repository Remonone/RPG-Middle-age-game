using RPG.Creatures.AI.Core;
using RPG.Inventories;
using RPG.Inventories.Items;
using RPG.Inventories.Pickups;
using RPG.Stats;
using UnityEngine;

namespace RPG.Creatures.AI.Actions {
    public class GetWeapon : GoapAction {

        [SerializeField] private float _searchRadius;
        [SerializeField] private BaseStats _stats;
        [SerializeField] private Equipment _equipment;

        private readonly LayerMask _pickupLayer = LayerMask.NameToLayer("Pickup");


        public override bool PerformAction(GameObject agent) {
            var pickup = Target.GetComponent<Pickup>();
            if (pickup == null) return true;
            var item = pickup.PickUp();
            var equipment = item.StoredItem as EquipmentItem;
            if (equipment == null) return true;
            _equipment.PlaceEquipment(equipment, EquipmentSlot.WEAPON);
            return true;
        }
        public override void DoReset() {
            Target = null;
        }
        public override bool IsDone() {
            return (Target.transform.position - transform.position).magnitude < _stats.GetStatValue(Stat.ATTACK_RANGE);
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
                if (!(localDistance < distance)) continue;
                Target = pickup.gameObject;
                distance = localDistance;
            }
            return true;
        }
        public override bool RequiresInRange() {
            return true;
        }
    }
}
