using UnityEngine;

namespace RPG.Combat {
    public record DamageReport {
        public float Damage;
        public DamageType Type;
        public GameObject Attacker;
    }
}
