using RPG.Combat;
using RPG.Creatures.AI.Core;
using RPG.Creatures.AI.Roles;
using RPG.Stats;
using UnityEngine;

namespace RPG.Creatures.AI.Actions {
    public class AttackPlayerWithWeapon : GoapAction {

        [SerializeField] private BaseStats _stats;
        
        private Health _targetToAttack;
        private Fighter _fighter;
        
        public AttackPlayerWithWeapon() {
            _prerequisites.Add(new StateObject {Name = "is_enemy_visible", Value = true});
            _prerequisites.Add(new StateObject {Name = "is_armed", Value = true});
            
            _effects.Add(new StateObject {Name = "liquidate_target", Value = true});
        }

        private void Awake() {
            _fighter = GetComponent<Fighter>();
        }

        public override bool PerformAction(GameObject agent) {
            var enemyTransform = _targetToAttack.gameObject.transform;
            if (!((enemyTransform.position - agent.transform.position).magnitude < _stats.GetStatValue(Stat.ATTACK_RANGE))) return false;
            _fighter.Attack(_targetToAttack);
            return _targetToAttack.IsAlive;
        }
        public override void DoReset() {
            InRange = false;
            Target = null;
        }
        public override bool IsDone() {
            return !_targetToAttack.IsAlive;
        }
        public override bool CheckProceduralPrerequisites(GameObject agent) {
            Target = agent.GetComponent<BaseAgentBehaviour>().CompletionTarget;
            if (!Target) return false;
            _targetToAttack = Target.GetComponent<Health>();
            return _targetToAttack;
        }
        public override bool RequiresInRange() {
            return true;
        }
        
    }
}
