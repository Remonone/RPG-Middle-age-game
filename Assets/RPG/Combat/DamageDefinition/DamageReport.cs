using UnityEngine;

namespace RPG.Combat.DamageDefinition {
    public record DamageReport {
        public float Damage;
        public DamageType Type;
        public GameObject Attacker;
        public GameObject Target;
    }
}
