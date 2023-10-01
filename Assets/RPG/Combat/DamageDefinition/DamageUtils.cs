using UnityEngine;

namespace RPG.Combat.DamageDefinition {
    public static class DamageUtils {
        public static DamageReport CreateReport(Health target, float damage, DamageType type, GameObject attacker) {
            if (target == null || !target.IsAlive ||
                attacker == null || damage <= 0) return null;
            var report = new DamageReport {
                Attacker = attacker,
                Type = type,
                Damage = damage
            };
            return report;
        }
    }
}
