using RPG.Core.Cursors;
using RPG.Creatures.Player;
using RPG.Stats.Relations;
using UnityEngine;

namespace RPG.Combat {
    public class SelectableTarget : MonoBehaviour, ITrajectory {
        [SerializeField] private bool _isTargetable;
        private OrganisationWrapper _wrapper;
        
        public bool Targetable => _isTargetable;
        
        private Health _health;

        private void Awake() {
            _health = GetComponent<Health>();
            _wrapper = GetComponent<OrganisationWrapper>();
        }

        public CursorType GetCursorType() {
            return CursorType.ATTACK;
        }

        public bool IsAggressiveTo(Organisation organisation) {
            var org = _wrapper.Organisation;
            var relations = org.GetRelationWithOrganisation(organisation);
            return relations < org.AgroThreshold;
        }
        
        
        public bool HandleRaycast(PlayerController invoker) {
            if (!enabled || !_isTargetable) return false;
            var fighter = invoker.GetComponent<Fighter>();
            if (_health is not {IsAlive: true}) return false;
            if (!IsAggressiveTo(invoker.GetComponent<OrganisationWrapper>().Organisation)) return false;
            if (invoker.Map["Action"].WasPressedThisFrame()) fighter.Attack(gameObject);
            return true;
        }
    }
}
