using RPG.Core.Cursors;
using RPG.Creatures.Player;
using RPG.Stats.Relations;
using UnityEngine;

namespace RPG.Combat {
    public class SelectableTarget : MonoBehaviour, ITrajectory {
        private bool _isTargetable;
        private IOrganisationWrapper _organisation;
        
        public bool Targetable => _isTargetable;
        
        private Health _health;

        private void Awake() {
            _health = GetComponent<Health>();
        }

        public CursorType GetCursorType() {
            return CursorType.ATTACK;
        }

        public bool IsAggressiveTo(Organisation organisation) {
            var org = _organisation.GetOrganisation();
            var relations = org.GetRelationWithOrganisation(organisation);
            return relations < org.AgroThreshold;
        }
        
        
        public bool HandleRaycast(PlayerController invoker) {
            if (!enabled || !_isTargetable) return false;
            var fighter = invoker.GetComponent<Fighter>();
            if (!fighter.CanAttack(_health)) return false;
            if (!IsAggressiveTo(invoker.GetOrganisation())) return false;
            if (invoker.Map["Action"].WasPressedThisFrame()) fighter.Attack(this);
            return true;
        }
    }
}
