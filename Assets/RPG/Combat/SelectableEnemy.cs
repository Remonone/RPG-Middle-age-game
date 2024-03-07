using System;
using RPG.Creatures.Player;
using RPG.UI.Cursors;
using UnityEngine;

namespace RPG.Combat {
    public class SelectableEnemy : MonoBehaviour, ITrajectory {
        public bool _isTargetable;
        public bool _isAggresive;

        private Health _health;

        private void Awake() {
            _health = GetComponent<Health>();
        }

        public CursorType GetCursorType() {
            return CursorType.ATTACK;
        }
        public bool HandleRaycast(PlayerController invoker) {
            if (enabled == false) return false;
            var fighter = invoker.GetComponent<Fighter>();
            if (!fighter.CanAttack(_health)) return false;
            if (invoker.Map["Action"].WasPressedThisFrame()) fighter.Attack(this);
            return true;
        }
    }
}
