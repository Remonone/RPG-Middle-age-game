using RPG.Combat.DamageDefinition;
using RPG.Inventories;
using RPG.Inventories.Items;
using RPG.Stats;
using Unity.Netcode;

namespace RPG.Combat {
    public class ServerFighter : Fighter {
        public override void Attack(SelectableTarget target) {
            if (!IsServer) return;
            if (!target.Targetable) return;
            var health = target.GetComponent<Health>();
            if (health is not { IsAlive: true }) return;
            Scheduler.SwitchAction(this);
            Target = health;
        }

        public override void Hit() {
            if (Target == null) return;
            if (!IsTargetInRange()) return;
            EquipmentItem weapon = Equipment.GetEquipmentItem(EquipmentSlot.WEAPON);
            DamageType type = weapon != null ? weapon.Type : DamageType.PHYSICAL;
            var report = DamageUtils.CreateReport(Target, Stats.GetStatValue(Stat.BASE_ATTACK), type, _networkReference); 
            InvokeOnAttackAction(report); // whenever cause attack to target, may invoke this event to give ability to handle some buffs or additional changes
            var netObject = Target.GetComponent<NetworkObject>();
            Target.HitEntity(report, netObject.NetworkObjectId);
            if (_shouldResetOnAttack) Scheduler.SwitchAction(null);
        }
    }
}
