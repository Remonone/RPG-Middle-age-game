using RPG.Combat;
using RPG.Creatures.AI.Core;
using RPG.Stats;
using UnityEngine;

namespace RPG.Creatures.AI.Actions {
    
    [RequireComponent(typeof(Fighter))]
    public class AttackPlayerWithBareHands : GoapAction {

        [SerializeField] private BaseStats _stats;
        [SerializeField] private AiVision _vision;

        private Health _targetToAttack;
        private Fighter _fighter;
        
        public AttackPlayerWithBareHands() {
            _prerequisites.Add(new StateObject {Name = "is_enemy_visible", Value = true});
            _prerequisites.Add(new StateObject {Name = "is_armed", Value = false});
            _prerequisites.Add(new StateObject {Name = "is_agro", Value = true });
            
            _effects.Add(new StateObject {Name = "liquidate_target", Value = true});
        }

        private void Awake() {
            _fighter = GetComponent<Fighter>();
        }

        public override bool PerformAction(GameObject agent) {
            var enemyTransform = _targetToAttack.gameObject.transform;
            if (!_vision.IsEnemiesInVision) return false;
            if (!((enemyTransform.position - agent.transform.position).magnitude >
                  _stats.GetStatValue(Stat.ATTACK_RANGE))) {
                InRange = false;
                return true;
            }
            _fighter.Attack(_targetToAttack);
            return true;
        }
        
        public override void DoReset() {
            InRange = false;
            Target = null;
            _targetToAttack = null;
        }
        
        public override bool IsDone() {
            return !_targetToAttack.IsAlive;
        }
        public override bool CheckProceduralPrerequisites(GameObject agent) {
            Target = _vision.EnemiesInVision.Current;
            if (!Target) return false;
            _targetToAttack = Target.GetComponent<Health>();
            return _targetToAttack != null && _targetToAttack.IsAlive;
        }
        
        public override bool RequiresInRange() {
            return true;
        }
    }
}
