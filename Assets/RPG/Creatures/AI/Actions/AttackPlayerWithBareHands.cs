using RPG.Combat;
using RPG.Creatures.AI.Core;
using RPG.Creatures.AI.Core.AgentBases;
using RPG.Stats;
using UnityEngine;

namespace RPG.Creatures.AI.Actions {
    
    [RequireComponent(typeof(Fighter))]
    public class AttackPlayerWithBareHands : GoapAction {

        [SerializeField] private BaseStats _stats;

        private Health _targetToAttack;
        private IFighterAgentBase _fighterAgent;
        private Fighter _fighter;
        
        public AttackPlayerWithBareHands() {
            _prerequisites.Add(new StateObject { Name = "is_enemy_visible", Value = true });
            _prerequisites.Add(new StateObject { Name = "is_armed", Value = false });
            _prerequisites.Add(new StateObject { Name = "is_agro", Value = true });
            
            _effects.Add(new StateObject { Name = "liquidate_target", Value = true });
        }

        private void Awake() {
            _fighter = GetComponent<Fighter>();
        }

        public override bool PerformAction(GameObject agent) {
            var enemy = _fighterAgent.GetEnemy();
            if (enemy == null) return false;
            if ((enemy.transform.position - agent.transform.position).magnitude >
                  _stats.GetStatValue(Stat.ATTACK_RANGE)) {
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
            _fighterAgent = null;
        }
        
        public override bool IsDone() {
            return !_targetToAttack.IsAlive;
        }
        public override bool CheckProceduralPrerequisites(GameObject agent) {
            if (!agent.TryGetComponent<IFighterAgentBase>(out var fighterAgent)) return false;
            _fighterAgent = fighterAgent;
            Target = _fighterAgent.GetEnemy();
            if (!Target) return false;
            _targetToAttack = Target.GetComponent<Health>();
            return _targetToAttack != null && _targetToAttack.IsAlive;
        }
        
        public override bool RequiresInRange() {
            return true;
        }
    }
}
