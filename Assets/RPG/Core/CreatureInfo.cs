using RPG.Combat;
using RPG.Movement;
using RPG.Stats;
using UnityEngine;

namespace RPG.Core {
    public class CreatureInfo : MonoBehaviour {
        private CreatureClass _class;
        private BaseStats _stats;
        private Health _health;
        private Mover _mover;

        public CreatureClass CreatureClass => _class;
        public Health Health => _health;
        public BaseStats Stats => _stats;
        public Mover Mover => _mover;
    }
}
