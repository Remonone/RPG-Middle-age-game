using Unity.Netcode;

namespace RPG.Combat.DamageDefinition {
    public static class DamageUtils {
        public static DamageReport CreateReport(Health target, float damage, DamageType type, NetworkObject attacker) {
            if (ReferenceEquals(target, null) || !target.IsAlive ||
                ReferenceEquals(attacker, null) || damage <= 0) return null;
            var report = new DamageReport {
                Attacker = attacker,
                Type = type,
                Damage = damage,
                Target = target.gameObject
            };
            return report;
        }
    }
}
